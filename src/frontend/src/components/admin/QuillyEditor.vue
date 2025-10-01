<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import Quill from "quill"; // Import Quill itself
import { QuillyEditor } from "vue-quilly";

import "quill/dist/quill.core.css";
import "quill/dist/quill.snow.css";

const props = defineProps({
  modelValue: {
    type: String,
    default: "",
  },
  options: {
    type: Object,
    default: () => ({}),
  },
  readOnly: {
    type: Boolean,
    default: false,
  },
  disable: {
    type: Boolean,
    default: false,
  },
  placeholder: {
    type: String,
    default: "Enter text here...",
  },
});

const emit = defineEmits(["update:modelValue", "ready"]);

const editorOptions = computed(() => {
  const defaultOptions = {
    theme: "snow",
    modules: {
      toolbar: [
        [{ header: [1, 2, 3, false] }],
        ["bold", "italic", "underline", "strike"],
        ["blockquote", "code-block"],
        [{ color: [] }, { background: [] }],
        [{ align: [] }],
        [{ list: "ordered" }, { list: "bullet" }],
        ["link", "image"],
        ["clean"],
      ],
    },
    placeholder: props.placeholder,
    readOnly: props.readOnly,
  };
  return { ...defaultOptions, ...props.options };
});

const editor = ref<InstanceType<typeof QuillyEditor>>();
let quillInstance: Quill | null = null;

const handleTextChange = (content: string) => {
  emit("update:modelValue", content);
};

onMounted(() => {
  if (editor.value) {
    quillInstance = editor.value.initialize(Quill);
    emit("ready", quillInstance);
  }
});

watch(
  () => props.readOnly,
  (isReadOnly) => {
    if (quillInstance) {
      if (isReadOnly) {
        quillInstance.disable();
      } else {
        quillInstance.enable();
      }
    }
  },
);
</script>

<template>
  <quilly-editor
    ref="editor"
    :options="editorOptions"
    :model-value="modelValue"
    @update:modelValue="handleTextChange"
  />
</template>
