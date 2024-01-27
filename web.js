/*
 * Copyright 2020 WebAssembly Community Group participants
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

const LAYOUT_CONFIG_KEY = 'layoutConfig';

const initialProgram =
`
let code () =
    let items = System.Collections.Generic.List<int>()
    for i in 1..10 do
        items.Add(i * 2)
    items.Add(11)
    printfn "hello"
    for i in 0..items.Length - 1 do
        printfn $"items[{i}] = {items[i]}"
code ()
`    

// Golden Layout
let layout = null;
let asmEditor = null;
window.asmEditor = asmEditor;
function AsmEditorComponent(container, state) {
  asmEditor = ace.edit(container.getElement()[0]);
  asmEditor.session.setMode('ace/mode/c_cpp');
  asmEditor.setOption('fontSize', `${state.fontSize || 18}px`);
  asmEditor.setReadOnly(true);

  container.on('fontSizeChanged', fontSize => {
    container.extendState({fontSize});
    asmEditor.setFontSize(`${fontSize}px`);
  });
  container.on('resize', debounceLazy(event => asmEditor.resize(), 20));
  container.on('destroy', event => {
    if (asmEditor) {
      asmEditor.destroy();
      asmEditor = null;
    }
  });
}

function initLayout() {
  const defaultLayoutConfig = {
    settings: {
      showCloseIcon: false,
      showPopoutIcon: false,
    },
    content: [{
      type: 'row',
      content: [{
        type: 'component',
        componentName: 'editor',
        componentState: {fontSize: 18, value: initialProgram},
      }, {
        type: 'column',
        content: [{
          type: 'component',
          componentName: 'Fable C',
          componentState: {fontSize: 18, value: 'asdf'}
        }, {
          type: 'component',
          componentName: 'terminal',
          componentState: {fontSize: 18},
        }]
      }]
    }]
  };

  layout = new Layout({
    configKey: LAYOUT_CONFIG_KEY,
    defaultLayoutConfig,
  });

  layout.on('initialised', event => {
    // Editor stuff
    editor.commands.addCommand({
      name: 'run',
      bindKey: {win: 'Ctrl+Enter', mac: 'Command+Enter'},
      exec: run
    });
  });

  layout.registerComponent('canvas', CanvasComponent);
  layout.registerComponent('Fable C', AsmEditorComponent);
  layout.init();
}

function resetLayout() {
  localStorage.removeItem('layoutConfig');
  if (layout) {
    layout.destroy();
    layout = null;
  }
  initLayout();
}

// Toolbar stuff
// $('#reset').on('click', event => { if (confirm('really reset?')) resetLayout() });
$('#run').on('click', event => {
  run(editor)
});


initLayout();
