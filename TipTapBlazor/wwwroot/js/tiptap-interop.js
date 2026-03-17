import {
    Editor,
    StarterKit,
    Placeholder,
    CharacterCount,
    Table,
    TableRow,
    TableCell,
    TableHeader,
    TaskList,
    TaskItem,
    Mention,
    TextAlign,
    Color,
    Highlight,
    FontFamily,
    Subscript,
    Superscript,
    Image,
    CodeBlockLowlight,
    Suggestion,
    computePosition,
    autoUpdate,
    offset,
    flip,
    shift,
    common,
    createLowlight,
} from './tiptap-bundle.js';

const editors = new Map();
const lowlight = createLowlight(common);

export function create(element, optionsJson, dotNetRef) {
    const options = JSON.parse(optionsJson);
    const id = element.id;

    if (editors.has(id)) {
        destroy(element);
    }

    const extensions = buildExtensions(options, dotNetRef);

    try {
        const editor = new Editor({
            element: element,
            extensions: extensions,
            content: options.initialContent || '',
            editable: !options.readOnly,
            onCreate: () => {
                updateActiveFormats(id, dotNetRef);
            },
            onUpdate: ({ editor }) => {
                const html = editor.getHTML();
                dotNetRef.invokeMethodAsync('OnContentChanged', html);
                if (editor.storage.characterCount) {
                    const count = editor.storage.characterCount.characters();
                    dotNetRef.invokeMethodAsync('OnCharacterCountChanged', count);
                }
            },
            onTransaction: () => {
                updateActiveFormats(id, dotNetRef);
            },
        });

        editors.set(id, { editor, dotNetRef });
    } catch (error) {
        console.error('TipTapBlazor: Failed to create editor', error);
    }
}

export function destroy(element) {
    const id = element.id;
    const entry = editors.get(id);
    if (entry) {
        entry.editor.destroy();
        editors.delete(id);
    }
}

export function getContent(element) {
    const entry = editors.get(element.id);
    return entry ? entry.editor.getHTML() : '';
}

export function setContent(element, html) {
    const entry = editors.get(element.id);
    if (entry) {
        entry.editor.commands.setContent(html, { emitUpdate: false });
    }
}

export function setEditable(element, editable) {
    const entry = editors.get(element.id);
    if (entry) {
        entry.editor.setEditable(editable);
    }
}

export function focus(element) {
    const entry = editors.get(element.id);
    if (entry) {
        entry.editor.commands.focus();
    }
}

export function executeCommand(element, command, argsJson) {
    const entry = editors.get(element.id);
    if (!entry) return;

    const args = argsJson ? JSON.parse(argsJson) : null;
    const { editor } = entry;

    switch (command) {
        case 'toggleBold':
            editor.chain().focus().toggleBold().run();
            break;
        case 'toggleItalic':
            editor.chain().focus().toggleItalic().run();
            break;
        case 'toggleUnderline':
            editor.chain().focus().toggleUnderline().run();
            break;
        case 'toggleStrike':
            editor.chain().focus().toggleStrike().run();
            break;
        case 'toggleCode':
            editor.chain().focus().toggleCode().run();
            break;
        case 'toggleSubscript':
            editor.chain().focus().toggleSubscript().run();
            break;
        case 'toggleSuperscript':
            editor.chain().focus().toggleSuperscript().run();
            break;
        case 'toggleBlockquote':
            editor.chain().focus().toggleBlockquote().run();
            break;
        case 'toggleBulletList':
            editor.chain().focus().toggleBulletList().run();
            break;
        case 'toggleOrderedList':
            editor.chain().focus().toggleOrderedList().run();
            break;
        case 'toggleTaskList':
            editor.chain().focus().toggleTaskList().run();
            break;
        case 'toggleCodeBlock':
            editor.chain().focus().toggleCodeBlock().run();
            break;
        case 'setHorizontalRule':
            editor.chain().focus().setHorizontalRule().run();
            break;
        case 'toggleHighlight':
            if (args && args.color) {
                editor.chain().focus().toggleHighlight({ color: args.color }).run();
            } else {
                editor.chain().focus().toggleHighlight().run();
            }
            break;
        case 'setColor':
            if (args && args.color) {
                editor.chain().focus().setColor(args.color).run();
            }
            break;
        case 'unsetColor':
            editor.chain().focus().unsetColor().run();
            break;
        case 'setFontFamily':
            if (args && args.fontFamily) {
                editor.chain().focus().setFontFamily(args.fontFamily).run();
            }
            break;
        case 'unsetFontFamily':
            editor.chain().focus().unsetFontFamily().run();
            break;
        case 'setHeading':
            if (args && args.level) {
                editor.chain().focus().toggleHeading({ level: args.level }).run();
            }
            break;
        case 'setParagraph':
            editor.chain().focus().setParagraph().run();
            break;
        case 'setTextAlign':
            if (args && args.align) {
                editor.chain().focus().setTextAlign(args.align).run();
            }
            break;
        case 'setLink':
            if (args && args.href) {
                editor.chain().focus().setLink({ href: args.href, target: args.target || null }).run();
            }
            break;
        case 'unsetLink':
            editor.chain().focus().unsetLink().run();
            break;
        case 'setImage':
            if (args && args.src) {
                editor.chain().focus().setImage({ src: args.src, alt: args.alt || '', title: args.title || '' }).run();
            }
            break;
        // Table commands
        case 'insertTable':
            editor.chain().focus().insertTable({
                rows: (args && args.rows) || 3,
                cols: (args && args.cols) || 3,
                withHeaderRow: true,
            }).run();
            break;
        case 'addRowBefore':
            editor.chain().focus().addRowBefore().run();
            break;
        case 'addRowAfter':
            editor.chain().focus().addRowAfter().run();
            break;
        case 'addColumnBefore':
            editor.chain().focus().addColumnBefore().run();
            break;
        case 'addColumnAfter':
            editor.chain().focus().addColumnAfter().run();
            break;
        case 'deleteRow':
            editor.chain().focus().deleteRow().run();
            break;
        case 'deleteColumn':
            editor.chain().focus().deleteColumn().run();
            break;
        case 'mergeCells':
            editor.chain().focus().mergeCells().run();
            break;
        case 'splitCell':
            editor.chain().focus().splitCell().run();
            break;
        case 'deleteTable':
            editor.chain().focus().deleteTable().run();
            break;
        // Undo/Redo
        case 'undo':
            editor.chain().focus().undo().run();
            break;
        case 'redo':
            editor.chain().focus().redo().run();
            break;
        default:
            console.warn(`TipTapBlazor: Unknown command '${command}'`);
    }
}

export function getActiveFormats(element) {
    const entry = editors.get(element.id);
    if (!entry) return '{}';
    return JSON.stringify(buildActiveFormats(entry.editor));
}

function updateActiveFormats(id, dotNetRef) {
    const entry = editors.get(id);
    if (!entry) return;
    const formats = buildActiveFormats(entry.editor);
    dotNetRef.invokeMethodAsync('OnSelectionChanged', JSON.stringify(formats));
}

function buildActiveFormats(editor) {
    return {
        bold: editor.isActive('bold'),
        italic: editor.isActive('italic'),
        underline: editor.isActive('underline'),
        strike: editor.isActive('strike'),
        code: editor.isActive('code'),
        subscript: editor.isActive('subscript'),
        superscript: editor.isActive('superscript'),
        blockquote: editor.isActive('blockquote'),
        bulletList: editor.isActive('bulletList'),
        orderedList: editor.isActive('orderedList'),
        taskList: editor.isActive('taskList'),
        codeBlock: editor.isActive('codeBlock'),
        heading1: editor.isActive('heading', { level: 1 }),
        heading2: editor.isActive('heading', { level: 2 }),
        heading3: editor.isActive('heading', { level: 3 }),
        heading4: editor.isActive('heading', { level: 4 }),
        link: editor.isActive('link'),
        highlight: editor.isActive('highlight'),
        alignLeft: editor.isActive({ textAlign: 'left' }),
        alignCenter: editor.isActive({ textAlign: 'center' }),
        alignRight: editor.isActive({ textAlign: 'right' }),
        alignJustify: editor.isActive({ textAlign: 'justify' }),
        table: editor.isActive('table'),
        fontFamily: editor.getAttributes('textStyle')?.fontFamily || '',
    };
}

function buildExtensions(options, dotNetRef) {
    const extensions = [];

    // StarterKit — always disable codeBlock here; CodeBlockLowlight replaces it when enabled
    const starterKitOptions = {
        codeBlock: false,
    };

    extensions.push(StarterKit.configure(starterKitOptions));

    if (options.enablePlaceholder && options.placeholder) {
        extensions.push(Placeholder.configure({ placeholder: options.placeholder }));
    }

    if (options.enableCharacterCount) {
        const ccOptions = {};
        if (options.maxLength > 0) {
            ccOptions.limit = options.maxLength;
        }
        extensions.push(CharacterCount.configure(ccOptions));
    }

    if (options.enableTable) {
        extensions.push(
            Table.configure({ resizable: true }),
            TableRow,
            TableCell,
            TableHeader,
        );
    }

    if (options.enableTaskList) {
        extensions.push(
            TaskList,
            TaskItem.configure({ nested: true }),
        );
    }

    if (options.enableTextAlign) {
        extensions.push(
            TextAlign.configure({ types: ['heading', 'paragraph'] }),
        );
    }

    if (options.enableColor) {
        extensions.push(Color);
    }

    if (options.enableHighlight) {
        extensions.push(Highlight.configure({ multicolor: true }));
    }

    if (options.enableFontFamily) {
        extensions.push(FontFamily);
    }

    if (options.enableSubscript) {
        extensions.push(Subscript);
    }

    if (options.enableSuperscript) {
        extensions.push(Superscript);
    }

    if (options.enableImage) {
        extensions.push(Image.configure({ inline: true }));
    }

    if (options.enableCodeBlock) {
        extensions.push(CodeBlockLowlight.configure({ lowlight }));
    }

    if (options.enableMention) {
        extensions.push(
            Mention.configure({
                HTMLAttributes: { class: 'ttp-mention' },
                suggestion: buildMentionSuggestion(dotNetRef),
            }),
        );
    }

    return extensions;
}

function buildMentionSuggestion(dotNetRef) {
    return {
        items: async ({ query }) => {
            if (!dotNetRef) return [];
            try {
                const result = await dotNetRef.invokeMethodAsync('OnMentionSearch', query);
                return result ? JSON.parse(result) : [];
            } catch (error) {
                console.warn('TipTapBlazor: Mention search failed', error);
                return [];
            }
        },
        render: () => {
            let popup = null;
            let list = null;
            let cleanupAutoUpdate = null;
            let items = [];
            let selectedIndex = 0;
            let currentCommand = null;

            return {
                onStart: (props) => {
                    items = props.items;
                    selectedIndex = 0;
                    currentCommand = props.command;

                    list = document.createElement('div');
                    list.className = 'ttp-mention-list';
                    renderList();

                    popup = document.createElement('div');
                    popup.className = 'ttp-mention-popup';
                    popup.appendChild(list);
                    document.body.appendChild(popup);

                    updatePosition(props);
                },
                onUpdate: (props) => {
                    items = props.items;
                    selectedIndex = 0;
                    currentCommand = props.command;
                    renderList();
                    updatePosition(props);
                },
                onKeyDown: (props) => {
                    if (props.event.key === 'ArrowUp') {
                        selectedIndex = (selectedIndex + items.length - 1) % items.length;
                        renderList();
                        return true;
                    }
                    if (props.event.key === 'ArrowDown') {
                        selectedIndex = (selectedIndex + 1) % items.length;
                        renderList();
                        return true;
                    }
                    if (props.event.key === 'Enter') {
                        const item = items[selectedIndex];
                        if (item && currentCommand) {
                            currentCommand(item);
                        }
                        return true;
                    }
                    return false;
                },
                onExit: () => {
                    if (cleanupAutoUpdate) {
                        cleanupAutoUpdate();
                        cleanupAutoUpdate = null;
                    }
                    if (popup && popup.parentNode) {
                        popup.parentNode.removeChild(popup);
                    }
                    popup = null;
                    list = null;
                    currentCommand = null;
                },
            };

            function renderList() {
                if (!list) return;
                list.replaceChildren();
                items.forEach((item, index) => {
                    const btn = document.createElement('button');
                    btn.className = 'ttp-mention-item' + (index === selectedIndex ? ' selected' : '');
                    btn.textContent = item.displayName || item.id;
                    if (item.category) {
                        btn.dataset.category = item.category;
                    }
                    btn.addEventListener('mousedown', (e) => {
                        e.preventDefault();
                    });
                    btn.addEventListener('click', () => {
                        if (currentCommand) {
                            currentCommand(item);
                        }
                        if (dotNetRef) {
                            dotNetRef.invokeMethodAsync('OnMentionSelected', JSON.stringify(item));
                        }
                    });
                    list.appendChild(btn);
                });
            }

            function updatePosition(props) {
                if (!popup || !props.clientRect) return;

                const virtualEl = {
                    getBoundingClientRect: props.clientRect,
                };

                if (cleanupAutoUpdate) {
                    cleanupAutoUpdate();
                }

                cleanupAutoUpdate = autoUpdate(virtualEl, popup, () => {
                    computePosition(virtualEl, popup, {
                        placement: 'bottom-start',
                        middleware: [offset(8), flip(), shift({ padding: 8 })],
                    }).then(({ x, y }) => {
                        Object.assign(popup.style, {
                            left: `${x}px`,
                            top: `${y}px`,
                            position: 'absolute',
                        });
                    });
                });
            }
        },
    };
}
