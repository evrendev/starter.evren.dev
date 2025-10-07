import Quill from "quill";
import "quill/dist/quill.core.css";
import "quill/dist/quill.snow.css";
import "quill-table-better/dist/quill-table-better.css";

import QuillResizeImage from "quill-resize-image";
import QuillTableBetter from "quill-table-better";

Quill.register("modules/resize", QuillResizeImage);
Quill.register(
  {
    "modules/table-better": QuillTableBetter,
  },
  true,
);
export default function () {}
