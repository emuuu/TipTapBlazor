// TipTap v3 extensions — rollup entry point
// Re-exports all extensions so tiptap-interop.js can import them from the bundle.

export { Editor } from '@tiptap/core';
export { StarterKit } from '@tiptap/starter-kit';

// Utility extensions (consolidated in v3)
export { Placeholder, CharacterCount } from '@tiptap/extensions';

// Table (consolidated in v3)
export { Table, TableRow, TableCell, TableHeader } from '@tiptap/extension-table';

// List extensions (consolidated in v3)
export { TaskList, TaskItem } from '@tiptap/extension-list';

// Individual extensions
export { Mention } from '@tiptap/extension-mention';
export { TextAlign } from '@tiptap/extension-text-align';
export { Color } from '@tiptap/extension-color';
export { Highlight } from '@tiptap/extension-highlight';
export { FontFamily } from '@tiptap/extension-font-family';
export { Subscript } from '@tiptap/extension-subscript';
export { Superscript } from '@tiptap/extension-superscript';
export { Image } from '@tiptap/extension-image';
export { CodeBlockLowlight } from '@tiptap/extension-code-block-lowlight';

// Suggestion plugin for mention
export { Suggestion } from '@tiptap/suggestion';

// Floating UI (replaces tippy.js in v3)
export { computePosition, autoUpdate, offset, flip, shift } from '@floating-ui/dom';

// Lowlight for code syntax highlighting
export { common, createLowlight } from 'lowlight';
