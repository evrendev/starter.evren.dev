// Downgrades a rich, positioned pptx-to-html render (Task F's isImported=true content —
// absolute-positioned text/images meant for a sandboxed iframe) into plain, linear
// Quill-compatible HTML: a flat flow of headings/paragraphs/images in DOM order, with all
// layout-purpose inline styles (position/top/left/width/height) stripped. One-way — the
// original layout is not recoverable from the output — the caller is expected to gate this
// behind an explicit, irreversible confirmation (see pages/admin/pages/form.vue).

const HEADING_MIN_FONT_SIZE = 20;

const parseFontSize = (style: string | null): number => {
  if (!style) return 0;
  const match = style.match(/font-size:\s*(\d+(?:\.\d+)?)px/i);
  return match ? parseFloat(match[1]) : 0;
};

const looksLikeHeading = (el: HTMLElement): boolean => {
  const style = el.getAttribute("style");
  const fontSize = parseFontSize(style);
  const isBold =
    /font-weight:\s*(bold|[6-9]00)/i.test(style ?? "") ||
    el.querySelector("b, strong") !== null;

  return fontSize >= HEADING_MIN_FONT_SIZE || (isBold && fontSize >= 16);
};

export function useConvertImportedContent() {
  const convertToEditable = (html: string): string => {
    if (!html || !html.trim()) return html;

    try {
      const doc = new DOMParser().parseFromString(html, "text/html");
      const output = document.createElement("div");

      let firstHeadingUsed = false;

      const appendText = (el: HTMLElement) => {
        const text = el.textContent?.trim();
        if (!text) return;

        const tag = looksLikeHeading(el)
          ? firstHeadingUsed
            ? "h3"
            : "h2"
          : "p";
        if (tag !== "p") firstHeadingUsed = true;

        const node = document.createElement(tag);
        node.textContent = text;
        output.appendChild(node);
      };

      const appendImage = (img: HTMLImageElement) => {
        const node = document.createElement("img");
        const src = img.getAttribute("src");
        const alt = img.getAttribute("alt");
        if (src) node.setAttribute("src", src);
        if (alt) node.setAttribute("alt", alt);
        output.appendChild(node);
      };

      // Walk the DOM in document order, emitting one flow-block per text-bearing
      // element and one <img> per image — skips containers themselves (their own
      // textContent would double-count descendants), only descends into ones that
      // have no direct text-bearing leaf and aren't images
      const isLeafTextElement = (el: HTMLElement): boolean =>
        Array.from(el.children).every(
          (child) => child.tagName === "BR" || child.tagName === "IMG",
        );

      const walk = (node: Element) => {
        for (const child of Array.from(node.children)) {
          if (child.tagName === "IMG") {
            appendImage(child as HTMLImageElement);
            continue;
          }

          const el = child as HTMLElement;
          const hasText = !!el.textContent?.trim();

          if (hasText && isLeafTextElement(el)) {
            appendText(el);
          } else {
            walk(el);
          }
        }
      };

      walk(doc.body);

      // Nothing recognizable was extracted (e.g. text sits directly on doc.body
      // with no wrapping element) — fall back to the original rather than
      // silently producing an empty page
      if (!output.innerHTML.trim()) return html;

      return output.innerHTML;
    } catch (error) {
      console.error(
        "Failed to convert imported content to editable HTML; keeping original content",
        error,
      );
      return html;
    }
  };

  return { convertToEditable };
}
