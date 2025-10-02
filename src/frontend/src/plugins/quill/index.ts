import Quill from "quill";
import "quill/dist/quill.core.css";
import "quill/dist/quill.snow.css";

import QuillResizeImage from "quill-resize-image";

Quill.register("modules/resize", QuillResizeImage);
export default function () {}
