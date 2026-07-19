import { pptxToHtml } from "@jvmr/pptx-to-html";

// Client-side, rich pptx->HTML rendering (positioned text/images/tables) via
// @jvmr/pptx-to-html, which relies on the browser's native DOMParser. That API is not
// exposed inside a dedicated Web Worker (confirmed empirically — see PPTX import Task F
// notes), so this deliberately runs on the main thread; `parsing` is exposed so callers
// can show a loading indicator instead of leaving the UI looking frozen on large decks.
// Failures never propagate — the backend falls back to its own plain-text OpenXml
// extraction whenever no (or no matching) client HTML is supplied for a slide.
export function usePptxSlidesParser() {
  const parsing = ref(false);
  const slidesHtml = ref<string[] | null>(null);

  const parse = async (file: File): Promise<string[] | null> => {
    parsing.value = true;
    slidesHtml.value = null;

    try {
      const buffer = await file.arrayBuffer();
      slidesHtml.value = await pptxToHtml(buffer, { width: 960, height: 540 });
      return slidesHtml.value;
    } catch (error) {
      console.error(
        "Client-side pptx parsing failed; import will proceed with the backend's own plain-text extraction",
        error,
      );
      slidesHtml.value = null;
      return null;
    } finally {
      parsing.value = false;
    }
  };

  const reset = () => {
    slidesHtml.value = null;
    parsing.value = false;
  };

  return { parsing, slidesHtml, parse, reset };
}
