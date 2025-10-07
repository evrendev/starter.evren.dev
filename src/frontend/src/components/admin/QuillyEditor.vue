<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { QuillyEditor } from "vue-quilly";
import Quill from "quill";

const { locale } = useI18n();
const languageMap: Record<string, string> = {
  tr: "tr_TR",
  en: "en_US",
  de: "de_DE",
};

const language = computed(() => languageMap[locale.value] || "en_US");

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
      table: false,
      toolbar: [
        [{ header: [1, 2, 3, false] }],
        ["bold", "italic", "underline", "strike"],
        ["blockquote", "code-block", "table-better"],
        [{ color: [] }, { background: [] }],
        [{ align: [] }],
        [{ list: "ordered" }, { list: "bullet" }],
        ["link", "image"],
        ["clean"],
      ],
      resize: {},
      "table-better": {
        language: language.value,
        menus: [
          "column",
          "row",
          "merge",
          "table",
          "cell",
          "wrap",
          "copy",
          "delete",
        ],
        toolbarTable: true,
      },
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

onMounted(async () => {
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

<style lang="scss">
.ql-editor {
  min-height: 240px;
}
</style>
