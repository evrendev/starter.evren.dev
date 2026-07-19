import http, { handleRequest } from "@/utils/http";
import type { AppError } from "@/primitives/error";

// Generic polling for any "background job with a status endpoint" — not tied to
// PPTX import specifically. Any future job (bulk export, video transcode, ...)
// can reuse this as long as its status DTO exposes `status` (+ optionally
// `percentComplete`) and its terminal states are string or numeric enum values.
export interface JobStatusLike {
  status: string | number;
  percentComplete?: number;
}

export interface UseJobPollingOptions {
  intervalMs?: number;
  isTerminal?: (status: string | number) => boolean;
}

const DEFAULT_TERMINAL_STATUSES: Array<string | number> = [
  "Completed",
  "Failed",
  2,
  3,
];

// `resolveStatusEndpoint` builds the status URL from a job id rather than taking a
// fixed URL up front: the job id is usually only known after an async action (e.g.
// a file upload) completes, but the composable itself still needs to be created
// synchronously during a component's setup() for its onUnmounted cleanup to attach.
// Call `start(jobId)` once the id is known to begin polling.
export function useJobPolling<T extends JobStatusLike>(
  resolveStatusEndpoint: (jobId: string) => string,
  options: UseJobPollingOptions = {},
) {
  const { intervalMs = 2000, isTerminal } = options;

  const jobId = ref<string | null>(null);
  const status = ref<T | null>(null);
  const error = ref<AppError | null>(null);

  const progress = computed(() => status.value?.percentComplete ?? 0);

  const checkTerminal = (value: T | null): boolean => {
    if (!value) return false;
    return isTerminal
      ? isTerminal(value.status)
      : DEFAULT_TERMINAL_STATUSES.includes(value.status);
  };

  const fetchStatus = async () => {
    if (!jobId.value) return;

    const result = await handleRequest<T>(
      http.get(resolveStatusEndpoint(jobId.value)),
    );

    if (result.succeeded && result.data) {
      status.value = result.data;
      error.value = null;
      if (checkTerminal(result.data)) pause();
    } else {
      error.value = result.errors ?? null;
      pause();
    }
  };

  const {
    pause,
    resume,
    isActive: isPolling,
  } = useIntervalFn(fetchStatus, intervalMs, { immediate: false });

  const start = async (id: string) => {
    jobId.value = id;
    status.value = null;
    error.value = null;
    await fetchStatus();
    if (!checkTerminal(status.value)) resume();
  };

  const reset = () => {
    pause();
    jobId.value = null;
    status.value = null;
    error.value = null;
  };

  onUnmounted(() => pause());

  return {
    jobId,
    status,
    error,
    progress,
    isPolling,
    start,
    stop: pause,
    reset,
  };
}
