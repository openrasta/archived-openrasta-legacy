// Script# Framework
// Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
// http://projects.nikhilk.net
//


Type.createNamespace('ScriptFX.UI');

////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AutoCompleteOptions

ScriptFX.UI.$create_AutoCompleteOptions = function ScriptFX_UI_AutoCompleteOptions(service) {
    var $o = { };
    Debug.assert(!String.isNullOrEmpty(service), 'service parameter must not be empty.');
    $o.service = service;
    $o.id = null;
    $o.itemCount = 10;
    $o.itemLookupDelay = 500;
    $o.minimumPrefixLength = 3;
    $o.cssClass = 'autoComplete';
    $o.itemCSSClass = 'autoCompleteItem';
    $o.selectedItemCSSClass = 'autoCompleteSelectedItem';
    $o.xOffset = 0;
    $o.yOffset = 0;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.EnterKeyOptions

ScriptFX.UI.$create_EnterKeyOptions = function ScriptFX_UI_EnterKeyOptions(clickTarget) {
    var $o = { };
    Debug.assert(clickTarget);
    $o.clickTarget = clickTarget;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.WatermarkOptions

ScriptFX.UI.$create_WatermarkOptions = function ScriptFX_UI_WatermarkOptions(watermarkText, watermarkCssClass) {
    var $o = { };
    $o.watermarkText = watermarkText;
    $o.watermarkCssClass = watermarkCssClass;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AutoCompleteBehavior

ScriptFX.UI.AutoCompleteBehavior = function ScriptFX_UI_AutoCompleteBehavior(domElement, options) {
    ScriptFX.UI.AutoCompleteBehavior.constructBase(this, [ domElement, options.id ]);
    this._options$1 = options;
    this._selectedItemIndex$1 = -1;
    domElement.autocomplete = 'off';
    var events = this.get_domEvents();
    events.attach('onfocus', Delegate.create(this, this._onTextBoxFocus$1));
    events.attach('onblur', Delegate.create(this, this._onTextBoxBlur$1));
    events.attach('onkeydown', Delegate.create(this, this._onTextBoxKeyDown$1));
}
ScriptFX.UI.AutoCompleteBehavior.prototype = {
    _options$1: null,
    _arguments$1: null,
    _dropDown$1: null,
    _dropDownPopup$1: null,
    _dropDownEvents$1: null,
    _dropDownVisible$1: false,
    _selectedItemIndex$1: 0,
    _itemCache$1: null,
    _timerID$1: 0,
    _currentPrefix$1: null,
    _itemRequest$1: null,
    
    get_arguments: function ScriptFX_UI_AutoCompleteBehavior$get_arguments() {
        if (!this._arguments$1) {
            this._arguments$1 = {};
        }
        return this._arguments$1;
    },
    
    add_itemDisplay: function ScriptFX_UI_AutoCompleteBehavior$add_itemDisplay(value) {
        this.get_events().addHandler('itemDisplay', value);
    },
    remove_itemDisplay: function ScriptFX_UI_AutoCompleteBehavior$remove_itemDisplay(value) {
        this.get_events().removeHandler('itemDisplay', value);
    },
    
    add_itemSelected: function ScriptFX_UI_AutoCompleteBehavior$add_itemSelected(value) {
        this.get_events().addHandler('itemSelected', value);
    },
    remove_itemSelected: function ScriptFX_UI_AutoCompleteBehavior$remove_itemSelected(value) {
        this.get_events().removeHandler('itemSelected', value);
    },
    
    add_requestingItems: function ScriptFX_UI_AutoCompleteBehavior$add_requestingItems(value) {
        this.get_events().addHandler('requestingItems', value);
    },
    remove_requestingItems: function ScriptFX_UI_AutoCompleteBehavior$remove_requestingItems(value) {
        this.get_events().removeHandler('requestingItems', value);
    },
    
    _abortRequest$1: function ScriptFX_UI_AutoCompleteBehavior$_abortRequest$1() {
        if (this._itemRequest$1) {
            this._itemRequest$1.abort();
            this._itemRequest$1 = null;
        }
    },
    
    clearCache: function ScriptFX_UI_AutoCompleteBehavior$clearCache() {
        this._itemCache$1 = null;
    },
    
    _createDropDown$1: function ScriptFX_UI_AutoCompleteBehavior$_createDropDown$1() {
        Debug.assert(!this._dropDown$1);
        this._dropDown$1 = document.createElement('DIV');
        if (this._options$1.cssClass) {
            this._dropDown$1.className = this._options$1.cssClass;
        }
        this._dropDown$1.unselectable = 'unselectable';
        document.body.appendChild(this._dropDown$1);
        this._dropDownEvents$1 = new ScriptFX.UI.DOMEventList(this._dropDown$1);
        this._dropDownEvents$1.attach('onmousedown', Delegate.create(this, this._onDropDownMouseDown$1));
        this._dropDownEvents$1.attach('onmouseup', Delegate.create(this, this._onDropDownMouseUp$1));
        this._dropDownEvents$1.attach('onmouseover', Delegate.create(this, this._onDropDownMouseOver$1));
        var options = ScriptFX.UI.$create_PopupOptions(this.get_domElement(), ScriptFX.UI.PopupMode.anchorBottomLeft);
        options.xOffset = this._options$1.xOffset;
        options.yOffset = -1 + this._options$1.yOffset;
        this._dropDownPopup$1 = new ScriptFX.UI.PopupBehavior(this._dropDown$1, options);
    },
    
    dispose: function ScriptFX_UI_AutoCompleteBehavior$dispose() {
        this._stopTimer$1();
        this._abortRequest$1();
        if (this._dropDown$1) {
            this._dropDownEvents$1.dispose();
            this._dropDownEvents$1 = null;
            this._dropDownPopup$1.dispose();
            this._dropDownPopup$1 = null;
            document.body.removeChild(this._dropDown$1);
            this._dropDown$1 = null;
            this._dropDownVisible$1 = false;
        }
        ScriptFX.UI.AutoCompleteBehavior.callBase(this, 'dispose');
    },
    
    _getDropDownItem$1: function ScriptFX_UI_AutoCompleteBehavior$_getDropDownItem$1(element) {
        while ((element) && (element !== this._dropDown$1)) {
            if (!isUndefined(element.__item)) {
                return element;
            }
            element = element.parentNode;
        }
        return null;
    },
    
    _hideDropDown$1: function ScriptFX_UI_AutoCompleteBehavior$_hideDropDown$1() {
        if (this._dropDownVisible$1) {
            this._dropDownVisible$1 = false;
            this._dropDownPopup$1.hide();
            this._selectedItemIndex$1 = -1;
        }
    },
    
    _highlightDropDownItem$1: function ScriptFX_UI_AutoCompleteBehavior$_highlightDropDownItem$1(itemElement) {
        if (this._options$1.selectedItemCSSClass) {
            ScriptFX.UI.Element.addCSSClass(itemElement, this._options$1.selectedItemCSSClass);
        }
    },
    
    _onDropDownMouseDown$1: function ScriptFX_UI_AutoCompleteBehavior$_onDropDownMouseDown$1() {
        var element = this._getDropDownItem$1(window.event.srcElement);
        if (element) {
            var item = element.__item;
            var index = element.__index;
            this._updateTextBox$1(item, index);
        }
    },
    
    _onDropDownMouseUp$1: function ScriptFX_UI_AutoCompleteBehavior$_onDropDownMouseUp$1() {
        this.get_domElement().focus();
    },
    
    _onDropDownMouseOver$1: function ScriptFX_UI_AutoCompleteBehavior$_onDropDownMouseOver$1() {
        var element = this._getDropDownItem$1(window.event.srcElement);
        if (this._selectedItemIndex$1 !== -1) {
            this._unhighlightItem$1(this._dropDown$1.childNodes[this._selectedItemIndex$1]);
            this._selectedItemIndex$1 = -1;
        }
        if (element) {
            var selectedItemIndex = element.__index;
            if (!isUndefined(this._selectedItemIndex$1)) {
                this._selectedItemIndex$1 = selectedItemIndex;
                this._highlightDropDownItem$1(element);
                return;
            }
        }
    },
    
    _onRequestComplete$1: function ScriptFX_UI_AutoCompleteBehavior$_onRequestComplete$1(request, context) {
        if ((request !== this._itemRequest$1) || (request.get_state() !== ScriptFX.Net.HTTPRequestState.completed) || (request.get_response().get_statusCode() !== ScriptFX.Net.HTTPStatusCode.OK)) {
            return;
        }
        var parameters = context;
        var prefixText = parameters['prefix'];
        var cacheKey = prefixText;
        if (this.get_events().getHandler('requestingItems')) {
            delete parameters.prefix;
            delete parameters.count;
            cacheKey += ScriptFX.JSON.serialize(parameters);
        }
        var items = request.get_response().getObject();
        this._updateDropDown$1(prefixText, items, cacheKey);
    },
    
    _onTextBoxBlur$1: function ScriptFX_UI_AutoCompleteBehavior$_onTextBoxBlur$1() {
        this._stopTimer$1();
        this._abortRequest$1();
        this._hideDropDown$1();
    },
    
    _onTextBoxFocus$1: function ScriptFX_UI_AutoCompleteBehavior$_onTextBoxFocus$1() {
        this._startTimer$1();
    },
    
    _onTextBoxKeyDown$1: function ScriptFX_UI_AutoCompleteBehavior$_onTextBoxKeyDown$1() {
        this._stopTimer$1();
        var e = window.event;
        if (this._dropDownVisible$1) {
            switch (e.keyCode) {
                case ScriptFX.UI.Key.escape:
                    this._hideDropDown$1();
                    e.returnValue = false;
                    break;
                case ScriptFX.UI.Key.up:
                    if (this._selectedItemIndex$1 > 0) {
                        this._unhighlightItem$1(this._dropDown$1.childNodes[this._selectedItemIndex$1]);
                        this._selectedItemIndex$1--;
                        this._highlightDropDownItem$1(this._dropDown$1.childNodes[this._selectedItemIndex$1]);
                    }
                    else if (this._selectedItemIndex$1 === -1) {
                        this._selectedItemIndex$1 = this._dropDown$1.childNodes.length - 1;
                        this._highlightDropDownItem$1(this._dropDown$1.childNodes[this._selectedItemIndex$1]);
                    }
                    e.returnValue = false;
                    break;
                case ScriptFX.UI.Key.down:
                    if (this._selectedItemIndex$1 < (this._dropDown$1.childNodes.length - 1)) {
                        if (this._selectedItemIndex$1 === -1) {
                            this._selectedItemIndex$1 = 0;
                        }
                        else {
                            this._unhighlightItem$1(this._dropDown$1.childNodes[this._selectedItemIndex$1]);
                            this._selectedItemIndex$1++;
                        }
                        this._highlightDropDownItem$1(this._dropDown$1.childNodes[this._selectedItemIndex$1]);
                    }
                    e.returnValue = false;
                    break;
                case ScriptFX.UI.Key.enter:
                    if (this._selectedItemIndex$1 !== -1) {
                        var item = this._dropDown$1.childNodes[this._selectedItemIndex$1].__item;
                        var index = this._dropDown$1.childNodes[this._selectedItemIndex$1].__index;
                        this._updateTextBox$1(item, index);
                    }
                    e.returnValue = false;
                    break;
            }
        }
        if (e.keyCode !== ScriptFX.UI.Key.tab) {
            this._startTimer$1();
        }
    },
    
    _onTimerTick$1: function ScriptFX_UI_AutoCompleteBehavior$_onTimerTick$1() {
        this._timerID$1 = 0;
        this._abortRequest$1();
        var text = (this.get_domElement()).value;
        if (text === this._currentPrefix$1) {
            return;
        }
        if (text.trim().length < this._options$1.minimumPrefixLength) {
            this._updateDropDown$1(null, null, null);
            return;
        }
        this._currentPrefix$1 = text;
        var requestHandler = this.get_events().getHandler('requestingItems');
        if (requestHandler) {
            var e = new ScriptFX.UI.AutoCompleteRequestEventArgs(text);
            requestHandler.invoke(this, e);
            var items = e.get__items();
            if (items) {
                this._updateDropDown$1(text, items, null);
                return;
            }
        }
        if (this._itemCache$1) {
            var cacheKey = text;
            if (this._arguments$1) {
                delete this._arguments$1.prefix;
                delete this._arguments$1.count;
                cacheKey += ScriptFX.JSON.serialize(this._arguments$1);
            }
            var items = this._itemCache$1[cacheKey];
            if (items) {
                this._updateDropDown$1(text, items, null);
                return;
            }
        }
        var parameters;
        if (this._arguments$1) {
            parameters = this._arguments$1;
        }
        else {
            parameters = {};
        }
        parameters['prefix'] = text;
        parameters['count'] = this._options$1.itemCount;
        this._itemRequest$1 = ScriptFX.Net.HTTPRequest.createRequest(ScriptFX.Net.HTTPRequest.createURI(this._options$1.service, parameters), ScriptFX.Net.HTTPVerb.GET);
        this._itemRequest$1.invoke(Delegate.create(this, this._onRequestComplete$1), parameters);
    },
    
    _showDropDown$1: function ScriptFX_UI_AutoCompleteBehavior$_showDropDown$1() {
        if (!this._dropDownVisible$1) {
            this._dropDownVisible$1 = true;
            this._dropDown$1.style.width = (this.get_domElement().offsetWidth - 2) + 'px';
            this._dropDownPopup$1.show();
        }
    },
    
    _startTimer$1: function ScriptFX_UI_AutoCompleteBehavior$_startTimer$1() {
        if (!this._timerID$1) {
            this._timerID$1 = window.setTimeout(Delegate.create(this, this._onTimerTick$1), this._options$1.itemLookupDelay);
        }
    },
    
    _stopTimer$1: function ScriptFX_UI_AutoCompleteBehavior$_stopTimer$1() {
        if (this._timerID$1) {
            window.clearTimeout(this._timerID$1);
            this._timerID$1 = 0;
        }
    },
    
    _unhighlightItem$1: function ScriptFX_UI_AutoCompleteBehavior$_unhighlightItem$1(itemElement) {
        if (this._options$1.selectedItemCSSClass) {
            ScriptFX.UI.Element.removeCSSClass(itemElement, this._options$1.selectedItemCSSClass);
        }
    },
    
    _updateDropDown$1: function ScriptFX_UI_AutoCompleteBehavior$_updateDropDown$1(prefixText, items, cacheKey) {
        var itemCount = 0;
        if (items) {
            itemCount = items.length;
        }
        if ((cacheKey) && (itemCount)) {
            if (!this._itemCache$1) {
                this._itemCache$1 = {};
            }
            this._itemCache$1[cacheKey] = items;
        }
        if (!this._dropDown$1) {
            this._createDropDown$1();
        }
        this._dropDown$1.innerHTML = '';
        this._selectedItemIndex$1 = -1;
        if (itemCount) {
            for (var i = 0; i < itemCount; i++) {
                var itemElement = document.createElement('DIV');
                if (this._options$1.itemCSSClass) {
                    itemElement.className = this._options$1.itemCSSClass;
                }
                var item = items[i];
                var text = item;
                var displayHandler = this.get_events().getHandler('itemDisplay');
                if (displayHandler) {
                    var e = new ScriptFX.UI.AutoCompleteItemEventArgs(item, i);
                    displayHandler.invoke(this, e);
                    text = e.get_text();
                    if (!text) {
                        text = item;
                    }
                }
                itemElement.innerHTML = text;
                itemElement.__index = i;
                itemElement.__item = items[i];
                this._dropDown$1.appendChild(itemElement);
            }
            this._showDropDown$1();
        }
        else {
            this._hideDropDown$1();
        }
    },
    
    _updateTextBox$1: function ScriptFX_UI_AutoCompleteBehavior$_updateTextBox$1(item, index) {
        this._stopTimer$1();
        this._hideDropDown$1();
        var text = null;
        var selectedHandler = this.get_events().getHandler('itemSelected');
        if (selectedHandler) {
            var e = new ScriptFX.UI.AutoCompleteItemEventArgs(item, index);
            selectedHandler.invoke(this, e);
            text = e.get_text();
        }
        if (!text) {
            text = item;
        }
        this._currentPrefix$1 = text;
        (this.get_domElement()).value = text;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AutoCompleteItemEventArgs

ScriptFX.UI.AutoCompleteItemEventArgs = function ScriptFX_UI_AutoCompleteItemEventArgs(item, index) {
    ScriptFX.UI.AutoCompleteItemEventArgs.constructBase(this);
    this._item$1 = item;
    this._index$1 = index;
}
ScriptFX.UI.AutoCompleteItemEventArgs.prototype = {
    _item$1: null,
    _index$1: 0,
    _text$1: null,
    
    get_index: function ScriptFX_UI_AutoCompleteItemEventArgs$get_index() {
        return this._index$1;
    },
    
    get_item: function ScriptFX_UI_AutoCompleteItemEventArgs$get_item() {
        return this._item$1;
    },
    
    get_text: function ScriptFX_UI_AutoCompleteItemEventArgs$get_text() {
        return this._text$1;
    },
    set_text: function ScriptFX_UI_AutoCompleteItemEventArgs$set_text(value) {
        this._text$1 = value;
        return value;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AutoCompleteRequestEventArgs

ScriptFX.UI.AutoCompleteRequestEventArgs = function ScriptFX_UI_AutoCompleteRequestEventArgs(prefixText) {
    ScriptFX.UI.AutoCompleteRequestEventArgs.constructBase(this);
    this._prefixText$1 = prefixText;
}
ScriptFX.UI.AutoCompleteRequestEventArgs.prototype = {
    _prefixText$1: null,
    _items$1: null,
    
    get__items: function ScriptFX_UI_AutoCompleteRequestEventArgs$get__items() {
        return this._items$1;
    },
    
    get_prefixText: function ScriptFX_UI_AutoCompleteRequestEventArgs$get_prefixText() {
        return this._prefixText$1;
    },
    
    setItems: function ScriptFX_UI_AutoCompleteRequestEventArgs$setItems(items) {
        this._items$1 = items;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Button

ScriptFX.UI.Button = function ScriptFX_UI_Button(domElement) {
    ScriptFX.UI.Button.constructBase(this, [ domElement ]);
    this.get_domEvents().attach('onclick', Delegate.create(this, this._onClick$2));
}
ScriptFX.UI.Button.prototype = {
    _actionArgument$2: null,
    _actionName$2: null,
    
    get_actionArgument: function ScriptFX_UI_Button$get_actionArgument() {
        return this._actionArgument$2;
    },
    set_actionArgument: function ScriptFX_UI_Button$set_actionArgument(value) {
        this._actionArgument$2 = value;
        return value;
    },
    
    get_actionName: function ScriptFX_UI_Button$get_actionName() {
        return this._actionName$2;
    },
    set_actionName: function ScriptFX_UI_Button$set_actionName(value) {
        this._actionName$2 = value;
        return value;
    },
    
    add_action: function ScriptFX_UI_Button$add_action(value) {
        this.get_events().addHandler(ScriptFX.UI.Button._clickEventKey$2, value);
    },
    remove_action: function ScriptFX_UI_Button$remove_action(value) {
        this.get_events().removeHandler(ScriptFX.UI.Button._clickEventKey$2, value);
    },
    
    add_click: function ScriptFX_UI_Button$add_click(value) {
        this.get_events().addHandler(ScriptFX.UI.Button._clickEventKey$2, value);
    },
    remove_click: function ScriptFX_UI_Button$remove_click(value) {
        this.get_events().removeHandler(ScriptFX.UI.Button._clickEventKey$2, value);
    },
    
    _onClick$2: function ScriptFX_UI_Button$_onClick$2() {
        var clickHandler = this.get_events().getHandler(ScriptFX.UI.Button._clickEventKey$2);
        if (clickHandler) {
            clickHandler.invoke(this, EventArgs.Empty);
        }
    },
    
    performClick: function ScriptFX_UI_Button$performClick() {
        this._onClick$2();
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.CheckBox

ScriptFX.UI.CheckBox = function ScriptFX_UI_CheckBox(domElement) {
    ScriptFX.UI.CheckBox.constructBase(this, [ domElement ]);
    this.get_domEvents().attach('onclick', Delegate.create(this, this._onClick$2));
}
ScriptFX.UI.CheckBox.prototype = {
    
    get_checked: function ScriptFX_UI_CheckBox$get_checked() {
        return (this.get_domElement()).checked;
    },
    set_checked: function ScriptFX_UI_CheckBox$set_checked(value) {
        (this.get_domElement()).checked = value;
        return value;
    },
    
    add_checkedChanged: function ScriptFX_UI_CheckBox$add_checkedChanged(value) {
        this.get_events().addHandler(ScriptFX.UI.CheckBox._checkChangedEventKey$2, value);
    },
    remove_checkedChanged: function ScriptFX_UI_CheckBox$remove_checkedChanged(value) {
        this.get_events().removeHandler(ScriptFX.UI.CheckBox._checkChangedEventKey$2, value);
    },
    
    _onClick$2: function ScriptFX_UI_CheckBox$_onClick$2() {
        var checkChangedHandler = this.get_events().getHandler(ScriptFX.UI.CheckBox._checkChangedEventKey$2);
        if (checkChangedHandler) {
            checkChangedHandler.invoke(this, EventArgs.Empty);
        }
        this.raisePropertyChanged('Checked');
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.EnterKeyBehavior

ScriptFX.UI.EnterKeyBehavior = function ScriptFX_UI_EnterKeyBehavior(element, options) {
    ScriptFX.UI.EnterKeyBehavior.constructBase(this, [ element, null ]);
    this._clickTarget$1 = options.clickTarget;
    this.get_domEvents().attach('onkeypress', Delegate.create(this, this._onElementKeyPress$1));
}
ScriptFX.UI.EnterKeyBehavior.prototype = {
    _clickTarget$1: null,
    
    _onElementKeyPress$1: function ScriptFX_UI_EnterKeyBehavior$_onElementKeyPress$1() {
        if ((window.event.keyCode === ScriptFX.UI.Key.enter) && (!this._clickTarget$1.disabled)) {
            window.event.cancelBubble = true;
            window.event.returnValue = false;
            this._clickTarget$1.click();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Label

ScriptFX.UI.Label = function ScriptFX_UI_Label(domElement) {
    ScriptFX.UI.Label.constructBase(this, [ domElement ]);
}
ScriptFX.UI.Label.prototype = {
    
    get_text: function ScriptFX_UI_Label$get_text() {
        return this.get_domElement().innerText;
    },
    set_text: function ScriptFX_UI_Label$set_text(value) {
        this.get_domElement().innerText = value;
        return value;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.TextBox

ScriptFX.UI.TextBox = function ScriptFX_UI_TextBox(domElement) {
    ScriptFX.UI.TextBox.constructBase(this, [ domElement ]);
    this.get_domEvents().attach('onchange', Delegate.create(this, this._onValueChanged$2));
}
ScriptFX.UI.TextBox.prototype = {
    
    get_text: function ScriptFX_UI_TextBox$get_text() {
        var watermark = ScriptFX.UI.Behavior.getBehavior(this.get_domElement(), ScriptFX.UI.WatermarkBehavior);
        if ((watermark) && watermark.get_isWatermarked()) {
            return String.Empty;
        }
        return (this.get_domElement()).value;
    },
    set_text: function ScriptFX_UI_TextBox$set_text(value) {
        (this.get_domElement()).value = value;
        this._onValueChanged$2();
        return value;
    },
    
    add_textChanged: function ScriptFX_UI_TextBox$add_textChanged(value) {
        this.get_events().addHandler(ScriptFX.UI.TextBox._textChangedEventKey$2, value);
    },
    remove_textChanged: function ScriptFX_UI_TextBox$remove_textChanged(value) {
        this.get_events().removeHandler(ScriptFX.UI.TextBox._textChangedEventKey$2, value);
    },
    
    _onValueChanged$2: function ScriptFX_UI_TextBox$_onValueChanged$2() {
        var textChangedHandler = this.get_events().getHandler(ScriptFX.UI.TextBox._textChangedEventKey$2);
        if (textChangedHandler) {
            textChangedHandler.invoke(this, EventArgs.Empty);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.WatermarkBehavior

ScriptFX.UI.WatermarkBehavior = function ScriptFX_UI_WatermarkBehavior(element, options) {
    ScriptFX.UI.WatermarkBehavior.constructBase(this, [ element, null ]);
    this._options$1 = options;
    this.get_domEvents().attach('onfocus', Delegate.create(this, this._onElementFocus$1));
    this.get_domEvents().attach('onblur', Delegate.create(this, this._onElementBlur$1));
    this.update();
}
ScriptFX.UI.WatermarkBehavior.prototype = {
    _options$1: null,
    _maxLength$1: 0,
    
    get_isWatermarked: function ScriptFX_UI_WatermarkBehavior$get_isWatermarked() {
        return ScriptFX.UI.Element.containsCSSClass(this.get_domElement(), this._options$1.watermarkCssClass);
    },
    
    _applyWatermark$1: function ScriptFX_UI_WatermarkBehavior$_applyWatermark$1() {
        var element = this.get_domElement();
        if (!element.value.length) {
            this._maxLength$1 = element.maxLength;
            element.maxLength = this._options$1.watermarkText.length;
            ScriptFX.UI.Element.addCSSClass(element, this._options$1.watermarkCssClass);
            element.value = this._options$1.watermarkText;
        }
    },
    
    _clearWatermark$1: function ScriptFX_UI_WatermarkBehavior$_clearWatermark$1(focusing) {
        var element = this.get_domElement();
        if (ScriptFX.UI.Element.containsCSSClass(element, this._options$1.watermarkCssClass)) {
            element.maxLength = this._maxLength$1;
            ScriptFX.UI.Element.removeCSSClass(element, this._options$1.watermarkCssClass);
            if (focusing) {
                element.value = '';
            }
        }
    },
    
    dispose: function ScriptFX_UI_WatermarkBehavior$dispose() {
        if (!this.get_isDisposed()) {
            this._clearWatermark$1(false);
        }
        ScriptFX.UI.WatermarkBehavior.callBase(this, 'dispose');
    },
    
    _onElementFocus$1: function ScriptFX_UI_WatermarkBehavior$_onElementFocus$1() {
        this._clearWatermark$1(true);
    },
    
    _onElementBlur$1: function ScriptFX_UI_WatermarkBehavior$_onElementBlur$1() {
        this._applyWatermark$1();
    },
    
    update: function ScriptFX_UI_WatermarkBehavior$update() {
        var element = this.get_domElement();
        if (!element.value.length) {
            this._applyWatermark$1();
        }
        else {
            this._clearWatermark$1(false);
        }
    }
}


ScriptFX.UI.AutoCompleteBehavior.createClass('ScriptFX.UI.AutoCompleteBehavior', ScriptFX.UI.Behavior);
ScriptFX.UI.AutoCompleteItemEventArgs.createClass('ScriptFX.UI.AutoCompleteItemEventArgs', EventArgs);
ScriptFX.UI.AutoCompleteRequestEventArgs.createClass('ScriptFX.UI.AutoCompleteRequestEventArgs', EventArgs);
ScriptFX.UI.Button.createClass('ScriptFX.UI.Button', ScriptFX.UI.Control, ScriptFX.UI.IAction);
ScriptFX.UI.CheckBox.createClass('ScriptFX.UI.CheckBox', ScriptFX.UI.Control, ScriptFX.UI.IToggle);
ScriptFX.UI.EnterKeyBehavior.createClass('ScriptFX.UI.EnterKeyBehavior', ScriptFX.UI.Behavior);
ScriptFX.UI.Label.createClass('ScriptFX.UI.Label', ScriptFX.UI.Control, ScriptFX.UI.IStaticText);
ScriptFX.UI.TextBox.createClass('ScriptFX.UI.TextBox', ScriptFX.UI.Control, ScriptFX.UI.IEditableText);
ScriptFX.UI.WatermarkBehavior.createClass('ScriptFX.UI.WatermarkBehavior', ScriptFX.UI.Behavior);
ScriptFX.UI.Button._clickEventKey$2 = 'click';
ScriptFX.UI.CheckBox._checkChangedEventKey$2 = 'checkChanged';
ScriptFX.UI.TextBox._textChangedEventKey$2 = 'textChanged';

// ---- Do not remove this footer ----
// Generated using Script# v0.5.1.0 (http://projects.nikhilk.net)
// -----------------------------------
