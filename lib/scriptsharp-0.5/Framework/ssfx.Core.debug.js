// Script# Framework
// Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
// http://projects.nikhilk.net
//


Type.createNamespace('ScriptFX');

////////////////////////////////////////////////////////////////////////////////
// ScriptFX.CollectionChangedAction

ScriptFX.CollectionChangedAction = function() { };
ScriptFX.CollectionChangedAction.prototype = {
    add: 0, 
    remove: 1, 
    reset: 2
}
ScriptFX.CollectionChangedAction.createEnum('ScriptFX.CollectionChangedAction', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX._registeredEvent

ScriptFX.$create__registeredEvent = function ScriptFX__registeredEvent(eventType, sender, eventArgs, eventCookie) {
    var $o = { };
    $o.eventType = eventType;
    $o.sender = sender;
    $o.eventArgs = eventArgs;
    $o.eventCookie = eventCookie;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.IEventManager

ScriptFX.IEventManager = function() { };
ScriptFX.IEventManager.prototype = {
    raiseEvent : null,
    registerEvent : null,
    registerEventHandler : null,
    unregisterEvent : null,
    unregisterEventHandler : null
}
ScriptFX.IEventManager.createInterface('ScriptFX.IEventManager');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.ISupportInitialize

ScriptFX.ISupportInitialize = function() { };
ScriptFX.ISupportInitialize.prototype = {
    beginInitialize : null,
    endInitialize : null
}
ScriptFX.ISupportInitialize.createInterface('ScriptFX.ISupportInitialize');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.INotifyDisposing

ScriptFX.INotifyDisposing = function() { };
ScriptFX.INotifyDisposing.prototype = {
    add_disposing : null,
    remove_disposing : null
}
ScriptFX.INotifyDisposing.createInterface('ScriptFX.INotifyDisposing');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.HostName

ScriptFX.HostName = function() { };
ScriptFX.HostName.prototype = {
    other: 0, 
    IE: 1, 
    mozilla: 2, 
    safari: 3, 
    opera: 4
}
ScriptFX.HostName.createEnum('ScriptFX.HostName', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.INotifyCollectionChanged

ScriptFX.INotifyCollectionChanged = function() { };
ScriptFX.INotifyCollectionChanged.prototype = {
    add_collectionChanged : null,
    remove_collectionChanged : null
}
ScriptFX.INotifyCollectionChanged.createInterface('ScriptFX.INotifyCollectionChanged');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.INotifyPropertyChanged

ScriptFX.INotifyPropertyChanged = function() { };
ScriptFX.INotifyPropertyChanged.prototype = {
    add_propertyChanged : null,
    remove_propertyChanged : null
}
ScriptFX.INotifyPropertyChanged.createInterface('ScriptFX.INotifyPropertyChanged');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.ITask

ScriptFX.ITask = function() { };
ScriptFX.ITask.prototype = {
    execute : null
}
ScriptFX.ITask.createInterface('ScriptFX.ITask');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.IObjectWithOwner

ScriptFX.IObjectWithOwner = function() { };
ScriptFX.IObjectWithOwner.prototype = {
    get_owner : null,
    setOwner : null
}
ScriptFX.IObjectWithOwner.createInterface('ScriptFX.IObjectWithOwner');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Application

ScriptFX.Application = function ScriptFX_Application() {
    this._disposableObjects = [];
    this._idleFrequency = 100;
    ScriptHost.add_load(Delegate.create(this, this._onScriptHostLoad));
    ScriptHost.add_unload(Delegate.create(this, this._onScriptHostUnload));
    this._windowUnloadingHandler = Delegate.create(this, this._onWindowUnloading);
    window.attachEvent('onbeforeunload', this._windowUnloadingHandler);
    this._windowErrorHandler = Delegate.create(this, this._onWindowError);
    window.attachEvent('onerror', this._windowErrorHandler);
    var rootElement = document.documentElement;
    var className = rootElement.className;
    if (className.startsWith('$')) {
        var hostInfo = this.get_host();
        className = className.replace('$browser', Enum.toString(ScriptFX.HostName, hostInfo.get_name()));
        className = className.replace('$majorver', hostInfo.get_majorVersion().toString());
        className = className.replace('$minorver', hostInfo.get_minorVersion().toString());
        rootElement.className = className;
    }
}
ScriptFX.Application.prototype = {
    _host: null,
    _isIE: 0,
    _scriptlets: null,
    _loaded: false,
    _disposing: false,
    _firstLoad: false,
    _sessionState: null,
    _history: null,
    _events: null,
    _disposableObjects: null,
    _idleFrequency: 0,
    _idleTimer: 0,
    _taskQueue: null,
    _taskTimer: 0,
    _registeredEventHandlers: null,
    _registeredEventTypes: null,
    _registeredEvents: null,
    _services: null,
    _windowUnloadingHandler: null,
    _windowErrorHandler: null,
    _idleTimerTickHandler: null,
    _taskTimerTickHandler: null,
    
    get_domain: function ScriptFX_Application$get_domain() {
        return window.document.domain;
    },
    set_domain: function ScriptFX_Application$set_domain(value) {
        window.document.domain = value;
        return value;
    },
    
    get__events: function ScriptFX_Application$get__events() {
        if (!this._events) {
            this._events = new ScriptFX.EventList();
        }
        return this._events;
    },
    
    get_history: function ScriptFX_Application$get_history() {
        Debug.assert(this._history, 'History has not been enabled.');
        return this._history;
    },
    
    get_host: function ScriptFX_Application$get_host() {
        if (!this._host) {
            this._host = new ScriptFX.HostInfo();
        }
        return this._host;
    },
    
    get_idleFrequency: function ScriptFX_Application$get_idleFrequency() {
        return this._idleFrequency;
    },
    set_idleFrequency: function ScriptFX_Application$set_idleFrequency(value) {
        Debug.assert(value >= 100, 'IdleFrequency must be atleast 100ms');
        this._idleFrequency = value;
        return value;
    },
    
    get_isFirstLoad: function ScriptFX_Application$get_isFirstLoad() {
        return this._firstLoad;
    },
    
    get_isIE: function ScriptFX_Application$get_isIE() {
        if (!this._isIE) {
            this._isIE = (this.get_host().get_name() === ScriptFX.HostName.IE) ? 1 : -1;
        }
        return (this._isIE === 1) ? true : false;
    },
    
    get_sessionState: function ScriptFX_Application$get_sessionState() {
        Debug.assert(this._loaded, 'You must wait until the load event before accessing session.');
        Debug.assert(this._sessionState, 'In order to use session, you must add an <input type=\"hidden\" id=\"__session\" /> within a <form>.');
        return this._sessionState;
    },
    
    add_error: function ScriptFX_Application$add_error(value) {
        this.get__events().addHandler('error', value);
    },
    remove_error: function ScriptFX_Application$remove_error(value) {
        this.get__events().removeHandler('error', value);
    },
    
    add_idle: function ScriptFX_Application$add_idle(value) {
        this.get__events().addHandler('idle', value);
        if (!this._idleTimer) {
            if (!this._idleTimerTickHandler) {
                this._idleTimerTickHandler = Delegate.create(this, this._onIdleTimerTick);
            }
            this._idleTimer = window.setTimeout(this._idleTimerTickHandler, this._idleFrequency);
        }
    },
    remove_idle: function ScriptFX_Application$remove_idle(value) {
        var isActive = this.get__events().removeHandler('idle', value);
        if ((!isActive) && (this._idleTimer)) {
            window.clearTimeout(this._idleTimer);
            this._idleTimer = 0;
        }
    },
    
    add_load: function ScriptFX_Application$add_load(value) {
        if (this._loaded) {
            value.invoke(this, EventArgs.Empty);
        }
        else {
            this.get__events().addHandler('load', value);
        }
    },
    remove_load: function ScriptFX_Application$remove_load(value) {
        this.get__events().removeHandler('load', value);
    },
    
    add_unload: function ScriptFX_Application$add_unload(value) {
        this.get__events().addHandler('unload', value);
    },
    remove_unload: function ScriptFX_Application$remove_unload(value) {
        this.get__events().removeHandler('unload', value);
    },
    
    add_unloading: function ScriptFX_Application$add_unloading(value) {
        this.get__events().addHandler('unloading', value);
    },
    remove_unloading: function ScriptFX_Application$remove_unloading(value) {
        this.get__events().removeHandler('unloading', value);
    },
    
    addTask: function ScriptFX_Application$addTask(task) {
        if (!this._taskQueue) {
            this._taskQueue = [];
        }
        this._taskQueue.enqueue(task);
        if (!this._taskTimer) {
            if (!this._taskTimerTickHandler) {
                this._taskTimerTickHandler = Delegate.create(this, this._onTaskTimerTick);
            }
            this._taskTimer = window.setTimeout(this._taskTimerTickHandler, 0);
        }
    },
    
    enableHistory: function ScriptFX_Application$enableHistory() {
        if (this._history) {
            return;
        }
        this._history = ScriptFX.HistoryManager._createHistory();
    },
    
    getService: function ScriptFX_Application$getService(serviceType) {
        Debug.assert(serviceType);
        if ((serviceType === IServiceContainer) || (serviceType === ScriptFX.IEventManager)) {
            return this;
        }
        if (this._services) {
            var name = serviceType.get_fullName().replace('.', '$');
            return this._services[name];
        }
        return null;
    },
    
    _onIdleTimerTick: function ScriptFX_Application$_onIdleTimerTick() {
        this._idleTimer = 0;
        var handler = this.get__events().getHandler('idle');
        if (handler) {
            handler.invoke(this, EventArgs.Empty);
            this._idleTimer = window.setTimeout(this._idleTimerTickHandler, this._idleFrequency);
        }
    },
    
    _onScriptHostLoad: function ScriptFX_Application$_onScriptHostLoad(sender, e) {
        var sessionElement = $('__session');
        if (sessionElement) {
            var value = sessionElement.value;
            if (String.isNullOrEmpty(value)) {
                this._firstLoad = true;
                this._sessionState = {};
            }
            else {
                this._sessionState = ScriptFX.JSON.deserialize(value);
                if (isUndefined(this._sessionState['__appLoaded'])) {
                    this._firstLoad = true;
                }
            }
            this._sessionState['__appLoaded'] = true;
        }
        else {
            this._firstLoad = true;
        }
        if (this._scriptlets) {
            for (var i = 0; i < this._scriptlets.length; i += 2) {
                this._scriptlets[i].main(this._scriptlets[i + 1]);
            }
            this._scriptlets = null;
        }
        this._loaded = true;
        var handler = this.get__events().getHandler('load');
        if (handler) {
            handler.invoke(this, EventArgs.Empty);
        }
        if (this._history) {
            this._history._initialize();
        }
    },
    
    _onScriptHostUnload: function ScriptFX_Application$_onScriptHostUnload(sender, e) {
        if (!this._disposing) {
            this._disposing = true;
            if (this._taskTimer) {
                window.clearTimeout(this._taskTimer);
            }
            if (this._idleTimer) {
                window.clearTimeout(this._idleTimer);
            }
            var handler = this.get__events().getHandler('unload');
            if (handler) {
                handler.invoke(this, EventArgs.Empty);
            }
            if (this._taskQueue) {
                while (this._taskQueue.length) {
                    var task = this._taskQueue.dequeue();
                    if (Type.canCast(task, IDisposable)) {
                        (task).dispose();
                    }
                }
            }
            if (this._disposableObjects.length) {
                var $enum1 = this._disposableObjects.getEnumerator();
                while ($enum1.moveNext()) {
                    var disposable = $enum1.get_current();
                    disposable.dispose();
                }
                this._disposableObjects.clear();
            }
            if (this._history) {
                this._history.dispose();
                this._history = null;
            }
            window.detachEvent('onbeforeunload', this._windowUnloadingHandler);
            window.detachEvent('onerror', this._windowErrorHandler);
            this._windowUnloadingHandler = null;
            this._windowErrorHandler = null;
            this._taskTimerTickHandler = null;
            this._idleTimerTickHandler = null;
        }
    },
    
    _onTaskTimerTick: function ScriptFX_Application$_onTaskTimerTick() {
        this._taskTimer = 0;
        if (this._taskQueue.length) {
            var task = this._taskQueue.dequeue();
            if (!task.execute()) {
                this._taskQueue.enqueue(task);
            }
            else {
                if (Type.canCast(task, IDisposable)) {
                    (task).dispose();
                }
            }
            if (this._taskQueue.length) {
                this._taskTimer = window.setTimeout(this._taskTimerTickHandler, 0);
            }
        }
    },
    
    _onWindowError: function ScriptFX_Application$_onWindowError() {
        var handler = this.get__events().getHandler('error');
        if (handler) {
            var ce = new ScriptFX.CancelEventArgs();
            ce.set_canceled(true);
            handler.invoke(this, ce);
            if (ce.get_canceled()) {
                window.event.returnValue = false;
            }
        }
    },
    
    _onWindowUnloading: function ScriptFX_Application$_onWindowUnloading() {
        window.event.avoidReturn = true;
        var handler = this.get__events().getHandler('unloading');
        if (handler) {
            var e = new ScriptFX.ApplicationUnloadingEventArgs();
            handler.invoke(this, e);
        }
        if (this._sessionState) {
            var sessionElement = $('__session');
            sessionElement.value = ScriptFX.JSON.serialize(this._sessionState);
        }
    },
    
    raiseEvent: function ScriptFX_Application$raiseEvent(eventType, sender, e) {
        Debug.assert(String.isNullOrEmpty(eventType));
        Debug.assert(sender);
        Debug.assert(e);
        if (this._registeredEventHandlers) {
            var handler = this._registeredEventHandlers[eventType];
            if (handler) {
                handler.invoke(sender, e);
            }
        }
    },
    
    registerDisposableObject: function ScriptFX_Application$registerDisposableObject(disposableObject) {
        if (!this._disposing) {
            this._disposableObjects.add(disposableObject);
        }
    },
    
    registerEvent: function ScriptFX_Application$registerEvent(eventType, sender, e) {
        Debug.assert(String.isNullOrEmpty(eventType));
        Debug.assert(sender);
        Debug.assert(e);
        if (this._registeredEventHandlers) {
            var handler = this._registeredEventHandlers[eventType];
            if (handler) {
                handler.invoke(sender, e);
            }
        }
        if (!this._registeredEvents) {
            this._registeredEvents = [];
        }
        if (!this._registeredEventTypes) {
            this._registeredEventTypes = {};
            this._registeredEventTypes[eventType] = 1;
        }
        else {
            var eventCount = this._registeredEventTypes[eventType];
            if (isUndefined(eventCount)) {
                this._registeredEventTypes[eventType] = 1;
            }
            else {
                this._registeredEventTypes[eventType] = 1 + eventCount;
            }
        }
        var eventInfo = ScriptFX.$create__registeredEvent(eventType, sender, e, this._registeredEvents.length);
        this._registeredEvents.add(eventInfo);
        return eventInfo.eventCookie;
    },
    
    registerEventHandler: function ScriptFX_Application$registerEventHandler(eventType, handler) {
        Debug.assert(!String.isNullOrEmpty(eventType));
        Debug.assert(handler);
        var existingHandler = null;
        if (!this._registeredEventHandlers) {
            this._registeredEventHandlers = {};
        }
        else {
            existingHandler = this._registeredEventHandlers[eventType];
        }
        this._registeredEventHandlers[eventType] = Delegate.combine(existingHandler, handler);
        if (!isNullOrUndefined(this._registeredEventTypes[eventType])) {
            var $enum1 = this._registeredEvents.getEnumerator();
            while ($enum1.moveNext()) {
                var eventInfo = $enum1.get_current();
                if (!eventInfo) {
                    continue;
                }
                if (eventInfo.eventType === eventType) {
                    handler.invoke(eventInfo.sender, eventInfo.eventArgs);
                }
            }
        }
    },
    
    registerService: function ScriptFX_Application$registerService(serviceType, service) {
        Debug.assert(serviceType);
        Debug.assert(service);
        if (!this._services) {
            this._services = {};
        }
        var name = serviceType.get_fullName().replace('.', '$');
        Debug.assert(!this._services[name]);
        this._services[name] = service;
    },
    
    run: function ScriptFX_Application$run(scriptletType, args) {
        if (this._loaded) {
            scriptletType.main(args);
        }
        else {
            if (!this._scriptlets) {
                this._scriptlets = [];
            }
            this._scriptlets.add(scriptletType);
            this._scriptlets.add(args);
        }
    },
    
    unregisterDisposableObject: function ScriptFX_Application$unregisterDisposableObject(disposableObject) {
        Debug.assert(disposableObject);
        if (!this._disposing) {
            this._disposableObjects.remove(disposableObject);
        }
    },
    
    unregisterEvent: function ScriptFX_Application$unregisterEvent(eventCookie) {
        Debug.assert(eventCookie);
        Debug.assert(Type.canCast(eventCookie, Number));
        Debug.assert(this._registeredEvents);
        Debug.assert(this._registeredEventTypes);
        var eventInfo = this._registeredEvents[eventCookie];
        Debug.assert(eventInfo);
        var eventCount = this._registeredEventTypes[eventInfo.eventType];
        Debug.assert(eventCount >= 1);
        if (eventCount === 1) {
            delete this._registeredEventTypes[eventInfo.eventType];
        }
        else {
            this._registeredEventTypes[eventInfo.eventType] = eventCount - 1;
        }
        this._registeredEvents[eventCookie] = null;
    },
    
    unregisterEventHandler: function ScriptFX_Application$unregisterEventHandler(eventType, handler) {
        Debug.assert(!String.isNullOrEmpty(eventType));
        Debug.assert(handler);
        if (this._registeredEventHandlers) {
            var existingHandler = this._registeredEventHandlers[eventType];
            if (existingHandler) {
                existingHandler = Delegate.remove(existingHandler, handler);
                if (!existingHandler) {
                    delete this._registeredEventHandlers[eventType];
                }
                else {
                    this._registeredEventHandlers[eventType] = existingHandler;
                }
            }
        }
    },
    
    unregisterService: function ScriptFX_Application$unregisterService(serviceType) {
        Debug.assert(serviceType);
        if (this._services) {
            var name = serviceType.get_fullName().replace('.', '$');
            delete this._services[name];
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.CancelEventArgs

ScriptFX.CancelEventArgs = function ScriptFX_CancelEventArgs() {
    ScriptFX.CancelEventArgs.constructBase(this);
}
ScriptFX.CancelEventArgs.prototype = {
    _canceled$1: false,
    
    get_canceled: function ScriptFX_CancelEventArgs$get_canceled() {
        return this._canceled$1;
    },
    set_canceled: function ScriptFX_CancelEventArgs$set_canceled(value) {
        this._canceled$1 = value;
        return value;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.CollectionChangedEventArgs

ScriptFX.CollectionChangedEventArgs = function ScriptFX_CollectionChangedEventArgs(action, item) {
    ScriptFX.CollectionChangedEventArgs.constructBase(this);
    this._action$1 = action;
    this._item$1 = item;
}
ScriptFX.CollectionChangedEventArgs.prototype = {
    _action$1: 0,
    _item$1: null,
    
    get_action: function ScriptFX_CollectionChangedEventArgs$get_action() {
        return this._action$1;
    },
    
    get_item: function ScriptFX_CollectionChangedEventArgs$get_item() {
        return this._item$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.ApplicationUnloadingEventArgs

ScriptFX.ApplicationUnloadingEventArgs = function ScriptFX_ApplicationUnloadingEventArgs() {
    ScriptFX.ApplicationUnloadingEventArgs.constructBase(this);
}
ScriptFX.ApplicationUnloadingEventArgs.prototype = {
    
    setUnloadPrompt: function ScriptFX_ApplicationUnloadingEventArgs$setUnloadPrompt(prompt) {
        window.event.returnValue = prompt;
        window.event.avoidReturn = false;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.HistoryManager

ScriptFX.HistoryManager = function ScriptFX_HistoryManager(enabled, iframe) {
    this._enabled = enabled;
    this._iframe = iframe;
}
ScriptFX.HistoryManager._createHistory = function ScriptFX_HistoryManager$_createHistory() {
    var hostName = ScriptFX.Application.current.get_host().get_name();
    if ((hostName !== ScriptFX.HostName.IE) && (hostName !== ScriptFX.HostName.mozilla)) {
        return new ScriptFX.HistoryManager(false, null);
    }
    var iframe = null;
    if (hostName === ScriptFX.HostName.IE) {
        iframe = $('_historyFrame');
        Debug.assert(iframe, 'You must have an <iframe id=\"_historyFrame\" src=\"Empty.htm\" /> tag on your page.');
    }
    return new ScriptFX.HistoryManager(true, iframe);
}
ScriptFX.HistoryManager.prototype = {
    _enabled: false,
    _iframe: null,
    _emptyPageURL: null,
    _iframeLoadHandler: null,
    _ignoreTimer: false,
    _ignoreIFrame: false,
    _currentEntry: null,
    
    get_isEnabled: function ScriptFX_HistoryManager$get_isEnabled() {
        return this._enabled;
    },
    
    add_navigated: function ScriptFX_HistoryManager$add_navigated(value) {
        this.__navigated = Delegate.combine(this.__navigated, value);
    },
    remove_navigated: function ScriptFX_HistoryManager$remove_navigated(value) {
        this.__navigated = Delegate.remove(this.__navigated, value);
    },
    
    __navigated: null,
    
    addEntry: function ScriptFX_HistoryManager$addEntry(entryName) {
        Debug.assert(!String.isNullOrEmpty(entryName));
        Debug.assert(!$(entryName), 'The entry identifier should not be the same as an element ID.');
        if (!this._enabled) {
            return;
        }
        this._ignoreTimer = true;
        if (this._iframe) {
            this._ignoreIFrame = true;
            this._iframe.src = this._emptyPageURL + entryName;
        }
        else {
            this._setCurrentEntry(entryName);
        }
    },
    
    dispose: function ScriptFX_HistoryManager$dispose() {
        if (this._iframe) {
            this._iframe.detachEvent('onload', this._iframeLoadHandler);
            this._iframe = null;
        }
    },
    
    _getCurrentEntry: function ScriptFX_HistoryManager$_getCurrentEntry() {
        var entryName = window.location.hash;
        if ((entryName.length) && (entryName.charAt(0) === '#')) {
            entryName = entryName.substr(1);
        }
        return entryName;
    },
    
    goBack: function ScriptFX_HistoryManager$goBack() {
        window.history.back();
    },
    
    goForward: function ScriptFX_HistoryManager$goForward() {
        window.history.forward();
    },
    
    _initialize: function ScriptFX_HistoryManager$_initialize() {
        if (!this._enabled) {
            return;
        }
        ScriptFX.Application.current.add_idle(Delegate.create(this, this._onAppIdle));
        if (this._iframe) {
            Debug.assert(this._iframe.src.length, 'You must set the Src attribute of the history iframe element to an empty page.');
            this._emptyPageURL = this._iframe.src + '?';
            this._iframeLoadHandler = Delegate.create(this, this._onIFrameLoad);
            this._iframe.attachEvent('onload', this._iframeLoadHandler);
        }
        this._currentEntry = this._getCurrentEntry();
        this._onNavigated(this._currentEntry);
    },
    
    _onAppIdle: function ScriptFX_HistoryManager$_onAppIdle(sender, e) {
        var entryName = this._getCurrentEntry();
        if (entryName !== this._currentEntry) {
            if (this._ignoreTimer) {
                return;
            }
            this._currentEntry = entryName;
            this._onNavigated(entryName);
        }
        else {
            this._ignoreTimer = false;
        }
    },
    
    _onIFrameLoad: function ScriptFX_HistoryManager$_onIFrameLoad() {
        var entryName = this._iframe.contentWindow.location.search;
        if ((entryName.length) && (entryName.charAt(0) === '?')) {
            entryName = entryName.substr(1);
        }
        this._setCurrentEntry(entryName);
        if (this._ignoreIFrame) {
            this._ignoreIFrame = false;
            return;
        }
        this._onNavigated(entryName);
    },
    
    _onNavigated: function ScriptFX_HistoryManager$_onNavigated(entryName) {
        if (this.__navigated) {
            this.__navigated.invoke(this, new ScriptFX.HistoryEventArgs(entryName));
        }
    },
    
    _setCurrentEntry: function ScriptFX_HistoryManager$_setCurrentEntry(entryName) {
        this._currentEntry = entryName;
        window.location.hash = entryName;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.HistoryEventArgs

ScriptFX.HistoryEventArgs = function ScriptFX_HistoryEventArgs(entryName) {
    ScriptFX.HistoryEventArgs.constructBase(this);
    this._entryName$1 = entryName;
}
ScriptFX.HistoryEventArgs.prototype = {
    _entryName$1: null,
    
    get_entryName: function ScriptFX_HistoryEventArgs$get_entryName() {
        return this._entryName$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.HostInfo

ScriptFX.HostInfo = function ScriptFX_HostInfo() {
    var userAgent = window.navigator.userAgent.toLowerCase();
    var version = null;
    var index;
    if ((index = userAgent.indexOf('opera')) >= 0) {
        this._name = ScriptFX.HostName.opera;
        version = userAgent.substr(index + 6);
    }
    else if ((index = userAgent.indexOf('msie')) >= 0) {
        this._name = ScriptFX.HostName.IE;
        version = userAgent.substr(index + 5);
    }
    else if ((index = userAgent.indexOf('safari')) >= 0) {
        this._name = ScriptFX.HostName.safari;
        version = userAgent.substr(index + 7);
    }
    else if ((index = userAgent.indexOf('firefox')) >= 0) {
        this._name = ScriptFX.HostName.mozilla;
        version = userAgent.substr(index + 8);
    }
    else if (userAgent.indexOf('gecko') >= 0) {
        this._name = ScriptFX.HostName.mozilla;
        version = window.navigator.appVersion;
    }
    if (version) {
        this._version = parseFloat(version);
        this._majorVersion = parseInt(this._version);
        if ((index = version.indexOf('.')) >= 0) {
            this._minorVersion = parseInt(version.substr(index + 1));
        }
    }
}
ScriptFX.HostInfo.prototype = {
    _name: 0,
    _version: 0,
    _majorVersion: 0,
    _minorVersion: 0,
    
    get_majorVersion: function ScriptFX_HostInfo$get_majorVersion() {
        return this._majorVersion;
    },
    
    get_minorVersion: function ScriptFX_HostInfo$get_minorVersion() {
        return this._minorVersion;
    },
    
    get_name: function ScriptFX_HostInfo$get_name() {
        return this._name;
    },
    
    get_version: function ScriptFX_HostInfo$get_version() {
        return this._version;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.EventList

ScriptFX.EventList = function ScriptFX_EventList() {
}
ScriptFX.EventList.prototype = {
    _events: null,
    
    addHandler: function ScriptFX_EventList$addHandler(key, handler) {
        Debug.assert(!String.isNullOrEmpty(key));
        Debug.assert(handler);
        if (!this._events) {
            this._events = {};
        }
        this._events[key] = Delegate.combine(this._events[key], handler);
    },
    
    getHandler: function ScriptFX_EventList$getHandler(key) {
        Debug.assert(!String.isNullOrEmpty(key));
        if (this._events) {
            return this._events[key];
        }
        return null;
    },
    
    removeHandler: function ScriptFX_EventList$removeHandler(key, handler) {
        Debug.assert(!String.isNullOrEmpty(key));
        Debug.assert(handler);
        if (this._events) {
            var sourceHandler = this._events[key];
            if (sourceHandler) {
                var newHandler = Delegate.remove(sourceHandler, handler);
                this._events[key] = newHandler;
                return (newHandler);
            }
        }
        return false;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.JSON

ScriptFX.JSON = function ScriptFX_JSON() {
}
ScriptFX.JSON.deserialize = function ScriptFX_JSON$deserialize(s) {
    if (String.isNullOrEmpty(s)) {
        return null;
    }
    if (!ScriptFX.JSON._dateRegex) {
        ScriptFX.JSON._dateRegex = new RegExp('(\'|\")\\\\@(-?[0-9]+)@(\'|\")', 'gm');
    }
    s = s.replace(ScriptFX.JSON._dateRegex, 'new Date($2)');
    return eval('(' + s + ')');
}
ScriptFX.JSON.serialize = function ScriptFX_JSON$serialize(o) {
    if (isNullOrUndefined(o)) {
        return String.Empty;
    }
    var sb = new StringBuilder();
    ScriptFX.JSON._serializeCore(sb, o);
    return sb.toString();
}
ScriptFX.JSON._serializeCore = function ScriptFX_JSON$_serializeCore(sb, o) {
    if (isNullOrUndefined(o)) {
        sb.append('null');
        return;
    }
    var scriptType = typeof(o);
    switch (scriptType) {
        case 'boolean':
            sb.append(o.toString());
            return;
        case 'number':
            sb.append((isFinite(o)) ? o.toString() : 'null');
            return;
        case 'string':
            sb.append((o).quote());
            return;
        case 'object':
            if (Array.isInstance(o)) {
                sb.append('[');
                var a = o;
                var length = a.length;
                var first = true;
                for (var i = 0; i < length; i++) {
                    if (first) {
                        first = false;
                    }
                    else {
                        sb.append(',');
                    }
                    ScriptFX.JSON._serializeCore(sb, a[i]);
                }
                sb.append(']');
            }
            else if (Date.isInstance(o)) {
                var d = o;
                var utcValue = Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(), d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds(), d.getUTCMilliseconds());
                sb.append('\"\\@');
                sb.append(utcValue.toString());
                sb.append('@\"');
            }
            else if (RegExp.isInstance(o)) {
                sb.append(o.toString());
            }
            else {
                sb.append('{');
                var first = true;
                var $dict1 = o;
                for (var $key2 in $dict1) {
                    var entry = { key: $key2, value: $dict1[$key2] };
                    if ((entry.key).startsWith('$') || Function.isInstance(entry.value)) {
                        continue;
                    }
                    if (first) {
                        first = false;
                    }
                    else {
                        sb.append(',');
                    }
                    sb.append(entry.key);
                    sb.append(':');
                    ScriptFX.JSON._serializeCore(sb, entry.value);
                }
                sb.append('}');
            }
            return;
        default:
            Debug.fail(scriptType + ' is not supported for JSON serialization.');
            sb.append('null');
            return;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.PropertyChangedEventArgs

ScriptFX.PropertyChangedEventArgs = function ScriptFX_PropertyChangedEventArgs(propertyName) {
    ScriptFX.PropertyChangedEventArgs.constructBase(this);
    this._propertyName$1 = propertyName;
}
ScriptFX.PropertyChangedEventArgs.prototype = {
    _propertyName$1: null,
    
    get_propertyName: function ScriptFX_PropertyChangedEventArgs$get_propertyName() {
        return this._propertyName$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.ObservableCollection

ScriptFX.ObservableCollection = function ScriptFX_ObservableCollection(owner, disposableItems) {
    this._owner = owner;
    this._items = [];
    this._disposableItems = disposableItems;
}
ScriptFX.ObservableCollection.prototype = {
    _owner: null,
    _items: null,
    _disposableItems: false,
    _handler: null,
    
    add_collectionChanged: function ScriptFX_ObservableCollection$add_collectionChanged(value) {
        this._handler = Delegate.combine(this._handler, value);
    },
    remove_collectionChanged: function ScriptFX_ObservableCollection$remove_collectionChanged(value) {
        this._handler = Delegate.remove(this._handler, value);
    },
    
    add: function ScriptFX_ObservableCollection$add(item) {
        (item).setOwner(this._owner);
        this._items.add(item);
        if (this._handler) {
            this._handler.invoke(this, new ScriptFX.CollectionChangedEventArgs(ScriptFX.CollectionChangedAction.add, item));
        }
    },
    
    clear: function ScriptFX_ObservableCollection$clear() {
        if (this._items.length) {
            var $enum1 = this._items.getEnumerator();
            while ($enum1.moveNext()) {
                var item = $enum1.get_current();
                item.setOwner(null);
            }
            this._items.clear();
            if (this._handler) {
                this._handler.invoke(this, new ScriptFX.CollectionChangedEventArgs(ScriptFX.CollectionChangedAction.reset, null));
            }
        }
    },
    
    contains: function ScriptFX_ObservableCollection$contains(item) {
        return this._items.contains(item);
    },
    
    dispose: function ScriptFX_ObservableCollection$dispose() {
        if (this._disposableItems) {
            var $enum1 = this._items.getEnumerator();
            while ($enum1.moveNext()) {
                var item = $enum1.get_current();
                item.dispose();
            }
        }
        this._items = null;
        this._owner = null;
        this._handler = null;
    },
    
    getEnumerator: function ScriptFX_ObservableCollection$getEnumerator() {
        return this._items.getEnumerator();
    },
    
    getItem: function ScriptFX_ObservableCollection$getItem(index) {
        return this._items[index];
    },
    
    getItems: function ScriptFX_ObservableCollection$getItems() {
        return this._items;
    },
    
    getLength: function ScriptFX_ObservableCollection$getLength() {
        return this._items.length;
    },
    
    remove: function ScriptFX_ObservableCollection$remove(item) {
        if (this._items.contains(item)) {
            (item).setOwner(null);
            this._items.remove(item);
            if (this._handler) {
                this._handler.invoke(this, new ScriptFX.CollectionChangedEventArgs(ScriptFX.CollectionChangedAction.remove, item));
            }
        }
    }
}


Type.createNamespace('ScriptFX.Net');

////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.HTTPStatusCode

ScriptFX.Net.HTTPStatusCode = function() { };
ScriptFX.Net.HTTPStatusCode.prototype = {
    canContinue: 100, 
    switchingProtocols: 101, 
    OK: 200, 
    created: 201, 
    partialContent: 206, 
    accepted: 202, 
    nonAuthoritativeInformation: 203, 
    noContent: 204, 
    resetContent: 205, 
    ambiguous: 300, 
    moved: 301, 
    redirect: 302, 
    redirectMethod: 303, 
    notModified: 304, 
    useProxy: 305, 
    temporaryRedirect: 307, 
    badRequest: 400, 
    methodNotAllowed: 400, 
    unauthorized: 401, 
    paymentRequired: 402, 
    forbidden: 403, 
    notFound: 404, 
    notAcceptable: 406, 
    proxyAuthenticationRequired: 407, 
    requestTimeout: 408, 
    conflict: 409, 
    gone: 410, 
    lengthRequired: 411, 
    preconditionFailed: 412, 
    requestEntityTooLarge: 413, 
    requestUriTooLong: 414, 
    unsupportedMediaType: 415, 
    requestedRangeNotSatisfiable: 416, 
    expectationFailed: 417, 
    internalServerError: 500, 
    notImplemented: 501, 
    badGateway: 502, 
    serviceUnavailable: 503, 
    gatewayTimeout: 504, 
    httpVersionNotSupported: 505
}
ScriptFX.Net.HTTPStatusCode.createEnum('ScriptFX.Net.HTTPStatusCode', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.HTTPRequestState

ScriptFX.Net.HTTPRequestState = function() { };
ScriptFX.Net.HTTPRequestState.prototype = {
    inactive: 0, 
    inProgress: 1, 
    completed: 2, 
    aborted: 3, 
    timedOut: 4
}
ScriptFX.Net.HTTPRequestState.createEnum('ScriptFX.Net.HTTPRequestState', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.HTTPVerb

ScriptFX.Net.HTTPVerb = function() { };
ScriptFX.Net.HTTPVerb.prototype = {
    GET: 0, 
    POST: 1, 
    PUT: 2, 
    DELETE: 3
}
ScriptFX.Net.HTTPVerb.createEnum('ScriptFX.Net.HTTPVerb', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.IHTTPResponse

ScriptFX.Net.IHTTPResponse = function() { };
ScriptFX.Net.IHTTPResponse.prototype = {
    get_contentLength : null,
    get_contentType : null,
    get_headers : null,
    get_request : null,
    get_statusCode : null,
    get_statusText : null,
    get_timeStamp : null,
    getHeader : null,
    getObject : null,
    getText : null,
    getXML : null
}
ScriptFX.Net.IHTTPResponse.createInterface('ScriptFX.Net.IHTTPResponse');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.HTTPRequest

ScriptFX.Net.HTTPRequest = function ScriptFX_Net_HTTPRequest() {
}
ScriptFX.Net.HTTPRequest.createRequest = function ScriptFX_Net_HTTPRequest$createRequest(uri, verb) {
    Debug.assert(!String.isNullOrEmpty(uri));
    var request = new ScriptFX.Net.HTTPRequest();
    if (!uri.startsWith('{')) {
        request._uri = uri;
    }
    else {
        var uriData = ScriptFX.JSON.deserialize(uri);
        request._uri = uriData['__uri'];
        Debug.assert(!String.isNullOrEmpty(request._uri));
        if (uriData['__nullParams']) {
            request._transportType = uriData['__transportType'];
        }
        else {
            request._transportType = Type.getType(uriData['__transportType']);
            delete uriData.__uri;
            delete uriData.__transportType;
            request._transportParameters = uriData;
        }
        Debug.assert((request._transportType) && ScriptFX.Net.HTTPTransport.isAssignableFrom(request._transportType));
    }
    request._verb = verb;
    return request;
}
ScriptFX.Net.HTTPRequest.createURI = function ScriptFX_Net_HTTPRequest$createURI(uri, parameters) {
    var sb = new StringBuilder(uri);
    if (uri.indexOf('?') < 0) {
        sb.append('?');
    }
    var parameterIndex = 0;
    var $dict1 = parameters;
    for (var $key2 in $dict1) {
        var entry = { key: $key2, value: $dict1[$key2] };
        if (parameterIndex) {
            sb.append('&');
        }
        sb.append(entry.key);
        sb.append('=');
        sb.append(encodeURIComponent(entry.value.toString()));
        parameterIndex++;
    }
    return sb.toString();
}
ScriptFX.Net.HTTPRequest.prototype = {
    _uri: null,
    _verb: 0,
    _content: null,
    _headers: null,
    _userName: null,
    _password: null,
    _transportType: null,
    _transportParameters: null,
    _timeout: 0,
    _callback: null,
    _context: null,
    _state: 0,
    _transport: null,
    _response: null,
    _timeStamp: null,
    
    get_content: function ScriptFX_Net_HTTPRequest$get_content() {
        return this._content;
    },
    set_content: function ScriptFX_Net_HTTPRequest$set_content(value) {
        Debug.assert(this.get_verb() === ScriptFX.Net.HTTPVerb.POST);
        Debug.assert(this._state === ScriptFX.Net.HTTPRequestState.inactive);
        this._content = value;
        return value;
    },
    
    get_hasCredentials: function ScriptFX_Net_HTTPRequest$get_hasCredentials() {
        return (!String.isNullOrEmpty(this._userName));
    },
    
    get_hasHeaders: function ScriptFX_Net_HTTPRequest$get_hasHeaders() {
        return (this._headers);
    },
    
    get_headers: function ScriptFX_Net_HTTPRequest$get_headers() {
        if (!this._headers) {
            this._headers = {};
        }
        return this._headers;
    },
    
    get_password: function ScriptFX_Net_HTTPRequest$get_password() {
        return this._password;
    },
    
    get_response: function ScriptFX_Net_HTTPRequest$get_response() {
        Debug.assert(this._state === ScriptFX.Net.HTTPRequestState.completed);
        return this._response;
    },
    
    get_state: function ScriptFX_Net_HTTPRequest$get_state() {
        return this._state;
    },
    
    get_timeout: function ScriptFX_Net_HTTPRequest$get_timeout() {
        return this._timeout;
    },
    set_timeout: function ScriptFX_Net_HTTPRequest$set_timeout(value) {
        this._timeout = value;
        return value;
    },
    
    get_timeStamp: function ScriptFX_Net_HTTPRequest$get_timeStamp() {
        return this._timeStamp;
    },
    
    get__transport: function ScriptFX_Net_HTTPRequest$get__transport() {
        return this._transport;
    },
    
    get__transportParameters: function ScriptFX_Net_HTTPRequest$get__transportParameters() {
        return this._transportParameters;
    },
    
    get_transportType: function ScriptFX_Net_HTTPRequest$get_transportType() {
        return this._transportType;
    },
    
    get_URI: function ScriptFX_Net_HTTPRequest$get_URI() {
        return this._uri;
    },
    
    get_userName: function ScriptFX_Net_HTTPRequest$get_userName() {
        return this._userName;
    },
    
    get_verb: function ScriptFX_Net_HTTPRequest$get_verb() {
        return this._verb;
    },
    
    abort: function ScriptFX_Net_HTTPRequest$abort() {
        if (this._state === ScriptFX.Net.HTTPRequestState.inProgress) {
            ScriptFX.Net.HTTPRequestManager._abort(this, false);
        }
    },
    
    dispose: function ScriptFX_Net_HTTPRequest$dispose() {
        if (this._transport) {
            this.abort();
        }
    },
    
    invoke: function ScriptFX_Net_HTTPRequest$invoke(callback, context) {
        Debug.assert(this._state === ScriptFX.Net.HTTPRequestState.inactive);
        this._callback = callback;
        this._context = context;
        ScriptFX.Application.current.registerDisposableObject(this);
        ScriptFX.Net.HTTPRequestManager._beginInvoke(this);
    },
    
    _invokeCallback: function ScriptFX_Net_HTTPRequest$_invokeCallback() {
        ScriptFX.Application.current.unregisterDisposableObject(this);
        if (this._transport) {
            this._transport.dispose();
            this._transport = null;
        }
        if (this._callback) {
            this._callback.invoke(this, this._context);
            this._callback = null;
            this._context = null;
        }
    },
    
    _onAbort: function ScriptFX_Net_HTTPRequest$_onAbort() {
        this._state = ScriptFX.Net.HTTPRequestState.aborted;
        this._invokeCallback();
    },
    
    _onActivate: function ScriptFX_Net_HTTPRequest$_onActivate(transport) {
        this._transport = transport;
        this._state = ScriptFX.Net.HTTPRequestState.inProgress;
        this._timeStamp = new Date();
    },
    
    _onCompleted: function ScriptFX_Net_HTTPRequest$_onCompleted(response) {
        this._response = response;
        this._state = ScriptFX.Net.HTTPRequestState.completed;
        this._invokeCallback();
    },
    
    _onTimeout: function ScriptFX_Net_HTTPRequest$_onTimeout() {
        this._state = ScriptFX.Net.HTTPRequestState.timedOut;
        this._invokeCallback();
    },
    
    setContentAsForm: function ScriptFX_Net_HTTPRequest$setContentAsForm(data) {
        Debug.assert(data);
        this.get_headers()['Content-Type'] = 'application/x-www-form-urlencoded';
        var sb = new StringBuilder();
        var firstValue = true;
        var $dict1 = data;
        for (var $key2 in $dict1) {
            var e = { key: $key2, value: $dict1[$key2] };
            if (!firstValue) {
                sb.append('&');
            }
            sb.append(e.key);
            sb.append('=');
            sb.append(encodeURIComponent(e.value.toString()));
            firstValue = false;
        }
        this.set_content(sb.toString());
    },
    
    setContentAsJSON: function ScriptFX_Net_HTTPRequest$setContentAsJSON(data) {
        Debug.assert(data);
        this.get_headers()['Content-Type'] = 'text/json';
        this.set_content(ScriptFX.JSON.serialize(data));
    },
    
    setCredentials: function ScriptFX_Net_HTTPRequest$setCredentials(userName, password) {
        Debug.assert(!String.isNullOrEmpty(userName));
        Debug.assert(!String.isNullOrEmpty(password));
        this._userName = userName;
        this._password = password;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.HTTPRequestManager

ScriptFX.Net.HTTPRequestManager = function ScriptFX_Net_HTTPRequestManager() {
}
ScriptFX.Net.HTTPRequestManager.add_requestInvoking = function ScriptFX_Net_HTTPRequestManager$add_requestInvoking(value) {
    ScriptFX.Net.HTTPRequestManager.__requestInvoking = Delegate.combine(ScriptFX.Net.HTTPRequestManager.__requestInvoking, value);
}
ScriptFX.Net.HTTPRequestManager.remove_requestInvoking = function ScriptFX_Net_HTTPRequestManager$remove_requestInvoking(value) {
    ScriptFX.Net.HTTPRequestManager.__requestInvoking = Delegate.remove(ScriptFX.Net.HTTPRequestManager.__requestInvoking, value);
}
ScriptFX.Net.HTTPRequestManager.add_requestInvoked = function ScriptFX_Net_HTTPRequestManager$add_requestInvoked(value) {
    ScriptFX.Net.HTTPRequestManager.__requestInvoked = Delegate.combine(ScriptFX.Net.HTTPRequestManager.__requestInvoked, value);
}
ScriptFX.Net.HTTPRequestManager.remove_requestInvoked = function ScriptFX_Net_HTTPRequestManager$remove_requestInvoked(value) {
    ScriptFX.Net.HTTPRequestManager.__requestInvoked = Delegate.remove(ScriptFX.Net.HTTPRequestManager.__requestInvoked, value);
}
ScriptFX.Net.HTTPRequestManager.get_online = function ScriptFX_Net_HTTPRequestManager$get_online() {
    return window.navigator.onLine;
}
ScriptFX.Net.HTTPRequestManager.get_timeoutInterval = function ScriptFX_Net_HTTPRequestManager$get_timeoutInterval() {
    return ScriptFX.Net.HTTPRequestManager._timeoutInterval;
}
ScriptFX.Net.HTTPRequestManager.set_timeoutInterval = function ScriptFX_Net_HTTPRequestManager$set_timeoutInterval(value) {
    ScriptFX.Net.HTTPRequestManager._timeoutInterval = value;
    return value;
}
ScriptFX.Net.HTTPRequestManager._abort = function ScriptFX_Net_HTTPRequestManager$_abort(request, timedOut) {
    var transport = request.get__transport();
    if (transport) {
        transport.abort();
        ScriptFX.Net.HTTPRequestManager._endInvoke(request, null, timedOut);
    }
}
ScriptFX.Net.HTTPRequestManager.abortAll = function ScriptFX_Net_HTTPRequestManager$abortAll() {
    var requests = ScriptFX.Net.HTTPRequestManager._activeRequests;
    ScriptFX.Net.HTTPRequestManager._activeRequests = [];
    var $enum1 = requests.getEnumerator();
    while ($enum1.moveNext()) {
        var request = $enum1.get_current();
        ScriptFX.Net.HTTPRequestManager._abort(request, false);
    }
}
ScriptFX.Net.HTTPRequestManager._beginInvoke = function ScriptFX_Net_HTTPRequestManager$_beginInvoke(request) {
    if (ScriptFX.Net.HTTPRequestManager.__requestInvoking) {
        var e = new ScriptFX.Net.PreHTTPRequestEventArgs(request);
        ScriptFX.Net.HTTPRequestManager.__requestInvoking.invoke(null, e);
        if (e.get_isSuppressed()) {
            request._onCompleted(e.get_response());
            return;
        }
    }
    var transportType = request.get_transportType();
    if (!transportType) {
        transportType = ScriptFX.Net._xmlhttpTransport;
    }
    var transport = new transportType(request);
    request._onActivate(transport);
    ScriptFX.Net.HTTPRequestManager._activeRequests.add(request);
    transport.invoke();
    if (((ScriptFX.Net.HTTPRequestManager._timeoutInterval) || (request.get_timeout())) && (!ScriptFX.Net.HTTPRequestManager._appIdleHandler)) {
        ScriptFX.Net.HTTPRequestManager._appIdleHandler = Delegate.create(null, ScriptFX.Net.HTTPRequestManager._onApplicationIdle);
        ScriptFX.Application.current.add_idle(ScriptFX.Net.HTTPRequestManager._appIdleHandler);
    }
}
ScriptFX.Net.HTTPRequestManager._endInvoke = function ScriptFX_Net_HTTPRequestManager$_endInvoke(request, response, timedOut) {
    ScriptFX.Net.HTTPRequestManager._activeRequests.remove(request);
    if (response) {
        request._onCompleted(response);
    }
    else if (timedOut) {
        request._onTimeout();
    }
    else {
        request._onAbort();
    }
    if (ScriptFX.Net.HTTPRequestManager.__requestInvoked) {
        var e = new ScriptFX.Net.PostHTTPRequestEventArgs(request, response);
        ScriptFX.Net.HTTPRequestManager.__requestInvoked.invoke(null, e);
    }
    if ((!ScriptFX.Net.HTTPRequestManager._activeRequests.length) && (ScriptFX.Net.HTTPRequestManager._appIdleHandler)) {
        ScriptFX.Application.current.remove_idle(ScriptFX.Net.HTTPRequestManager._appIdleHandler);
        ScriptFX.Net.HTTPRequestManager._appIdleHandler = null;
    }
}
ScriptFX.Net.HTTPRequestManager._onApplicationIdle = function ScriptFX_Net_HTTPRequestManager$_onApplicationIdle(sender, e) {
    if (!ScriptFX.Net.HTTPRequestManager._activeRequests.length) {
        return;
    }
    var timedOutRequests = null;
    var currentTimeValue = (new Date()).getTime();
    var $enum1 = ScriptFX.Net.HTTPRequestManager._activeRequests.getEnumerator();
    while ($enum1.moveNext()) {
        var request = $enum1.get_current();
        var timeStampValue = request.get_timeStamp().getTime();
        var interval = request.get_timeout();
        if (!interval) {
            interval = ScriptFX.Net.HTTPRequestManager._timeoutInterval;
            if (!interval) {
                continue;
            }
        }
        if ((currentTimeValue - timeStampValue) > interval) {
            if (!timedOutRequests) {
                timedOutRequests = [];
            }
            timedOutRequests.add(request);
        }
    }
    if (timedOutRequests) {
        var $enum2 = timedOutRequests.getEnumerator();
        while ($enum2.moveNext()) {
            var request = $enum2.get_current();
            ScriptFX.Net.HTTPRequestManager._abort(request, true);
        }
    }
}
ScriptFX.Net.HTTPRequestManager._onCompleted = function ScriptFX_Net_HTTPRequestManager$_onCompleted(request, response) {
    ScriptFX.Net.HTTPRequestManager._endInvoke(request, response, false);
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.HTTPTransport

ScriptFX.Net.HTTPTransport = function ScriptFX_Net_HTTPTransport(request) {
    this._request = request;
}
ScriptFX.Net.HTTPTransport.createURI = function ScriptFX_Net_HTTPTransport$createURI(uri, transportType, parameters) {
    Debug.assert(!String.isNullOrEmpty(uri));
    Debug.assert((transportType) && ScriptFX.Net.HTTPTransport.isAssignableFrom(transportType));
    if (!parameters) {
        return '{__nullParams: true, __uri:\'' + uri + '\', __transportType: ' + transportType.get_fullName() + '}';
    }
    else {
        parameters['__uri'] = uri;
        parameters['__transportType'] = transportType.get_fullName();
        return ScriptFX.JSON.serialize(parameters);
    }
}
ScriptFX.Net.HTTPTransport.prototype = {
    _request: null,
    
    get_parameters: function ScriptFX_Net_HTTPTransport$get_parameters() {
        return this._request.get__transportParameters();
    },
    
    get_request: function ScriptFX_Net_HTTPTransport$get_request() {
        return this._request;
    },
    
    getMethod: function ScriptFX_Net_HTTPTransport$getMethod() {
        return Enum.toString(ScriptFX.Net.HTTPVerb, this._request.get_verb());
    },
    
    onCompleted: function ScriptFX_Net_HTTPTransport$onCompleted(response) {
        ScriptFX.Net.HTTPRequestManager._onCompleted(this._request, response);
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.PostHTTPRequestEventArgs

ScriptFX.Net.PostHTTPRequestEventArgs = function ScriptFX_Net_PostHTTPRequestEventArgs(request, response) {
    ScriptFX.Net.PostHTTPRequestEventArgs.constructBase(this);
    this._request$1 = request;
    this._response$1 = response;
}
ScriptFX.Net.PostHTTPRequestEventArgs.prototype = {
    _request$1: null,
    _response$1: null,
    
    get_request: function ScriptFX_Net_PostHTTPRequestEventArgs$get_request() {
        return this._request$1;
    },
    
    get_response: function ScriptFX_Net_PostHTTPRequestEventArgs$get_response() {
        return this._response$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.PreHTTPRequestEventArgs

ScriptFX.Net.PreHTTPRequestEventArgs = function ScriptFX_Net_PreHTTPRequestEventArgs(request) {
    ScriptFX.Net.PreHTTPRequestEventArgs.constructBase(this);
    this._request$1 = request;
}
ScriptFX.Net.PreHTTPRequestEventArgs.prototype = {
    _request$1: null,
    _response$1: null,
    _suppressed$1: false,
    
    get_isSuppressed: function ScriptFX_Net_PreHTTPRequestEventArgs$get_isSuppressed() {
        return this._suppressed$1;
    },
    
    get_request: function ScriptFX_Net_PreHTTPRequestEventArgs$get_request() {
        return this._request$1;
    },
    
    get_response: function ScriptFX_Net_PreHTTPRequestEventArgs$get_response() {
        return this._response$1;
    },
    
    suppressRequest: function ScriptFX_Net_PreHTTPRequestEventArgs$suppressRequest(response) {
        this._suppressed$1 = true;
        this._response$1 = response;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net._xmlhttpResponse

ScriptFX.Net._xmlhttpResponse = function ScriptFX_Net__xmlhttpResponse(request, xmlHTTP) {
    this._timeStamp = new Date();
    this._request = request;
    this._xmlHTTP = xmlHTTP;
}
ScriptFX.Net._xmlhttpResponse.prototype = {
    _request: null,
    _xmlHTTP: null,
    _headers: null,
    _timeStamp: null,
    _text: null,
    _object: null,
    _xml: null,
    
    get_contentLength: function ScriptFX_Net__xmlhttpResponse$get_contentLength() {
        return this.getText().length;
    },
    
    get_contentType: function ScriptFX_Net__xmlhttpResponse$get_contentType() {
        return this._xmlHTTP.getResponseHeader('Content-Type');
    },
    
    get_headers: function ScriptFX_Net__xmlhttpResponse$get_headers() {
        if (!this._headers) {
            var headers = this._xmlHTTP.getAllResponseHeaders();
            var parts = headers.split('\n');
            this._headers = {};
            var $enum1 = parts.getEnumerator();
            while ($enum1.moveNext()) {
                var part = $enum1.get_current();
                var colonIndex = part.indexOf(':');
                this._headers[part.substr(0, colonIndex)] = part.substr(colonIndex + 1).trim();
            }
        }
        return this._headers;
    },
    
    get_request: function ScriptFX_Net__xmlhttpResponse$get_request() {
        return this._request;
    },
    
    get_statusCode: function ScriptFX_Net__xmlhttpResponse$get_statusCode() {
        return this._xmlHTTP.status;
    },
    
    get_statusText: function ScriptFX_Net__xmlhttpResponse$get_statusText() {
        return this._xmlHTTP.statusText;
    },
    
    get_timeStamp: function ScriptFX_Net__xmlhttpResponse$get_timeStamp() {
        return this._timeStamp;
    },
    
    getHeader: function ScriptFX_Net__xmlhttpResponse$getHeader(name) {
        return this._xmlHTTP.getResponseHeader(name);
    },
    
    getObject: function ScriptFX_Net__xmlhttpResponse$getObject() {
        if (!this._object) {
            this._object = ScriptFX.JSON.deserialize(this.getText());
        }
        return this._object;
    },
    
    getText: function ScriptFX_Net__xmlhttpResponse$getText() {
        if (!this._text) {
            this._text = this._xmlHTTP.responseText;
        }
        return this._text;
    },
    
    getXML: function ScriptFX_Net__xmlhttpResponse$getXML() {
        if (!this._xml) {
            var xml = this._xmlHTTP.responseXML;
            if ((!xml) || (!xml.documentElement)) {
                try {
                    xml = XMLDocumentParser.parse(this._xmlHTTP.responseText);
                    if ((xml) && (xml.documentElement)) {
                        this._xml = xml;
                    }
                }
                catch ($e1) {
                }
            }
            else {
                this._xml = xml;
                if (ScriptFX.Application.current.get_isIE()) {
                    xml.setProperty('SelectionLanguage', 'XPath');
                }
            }
        }
        return this._xml;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net._xmlhttpTransport

ScriptFX.Net._xmlhttpTransport = function ScriptFX_Net__xmlhttpTransport(request) {
    ScriptFX.Net._xmlhttpTransport.constructBase(this, [ request ]);
}
ScriptFX.Net._xmlhttpTransport.prototype = {
    _xmlHTTP$1: null,
    
    abort: function ScriptFX_Net__xmlhttpTransport$abort() {
        if (this._xmlHTTP$1) {
            this._xmlHTTP$1.onreadystatechange = Delegate.Null;
            this._xmlHTTP$1.abort();
            this._xmlHTTP$1 = null;
        }
    },
    
    dispose: function ScriptFX_Net__xmlhttpTransport$dispose() {
        this.abort();
    },
    
    invoke: function ScriptFX_Net__xmlhttpTransport$invoke() {
        var request = this.get_request();
        this._xmlHTTP$1 = new XMLHttpRequest();
        this._xmlHTTP$1.onreadystatechange = Delegate.create(this, this._onReadyStateChange$1);
        if (!this.get_request().get_hasCredentials()) {
            this._xmlHTTP$1.open(this.getMethod(), request.get_URI(), true);
        }
        else {
            this._xmlHTTP$1.open(this.getMethod(), request.get_URI(), true, request.get_userName(), request.get_password());
        }
        var headers = (request.get_hasHeaders()) ? request.get_headers() : null;
        if (headers) {
            var $dict1 = headers;
            for (var $key2 in $dict1) {
                var entry = { key: $key2, value: $dict1[$key2] };
                this._xmlHTTP$1.setRequestHeader(entry.key, entry.value);
            }
        }
        var body = request.get_content();
        if ((body) && ((!headers) || (!headers['Content-Type']))) {
            this._xmlHTTP$1.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        }
        this._xmlHTTP$1.send(body);
    },
    
    _onReadyStateChange$1: function ScriptFX_Net__xmlhttpTransport$_onReadyStateChange$1() {
        if (this._xmlHTTP$1.readyState === 4) {
            var response = new ScriptFX.Net._xmlhttpResponse(this.get_request(), this._xmlHTTP$1);
            this._xmlHTTP$1.onreadystatechange = Delegate.Null;
            this._xmlHTTP$1 = null;
            this.onCompleted(response);
        }
    }
}


Type.createNamespace('ScriptFX.UI');

////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AnimationStopState

ScriptFX.UI.AnimationStopState = function() { };
ScriptFX.UI.AnimationStopState.prototype = {
    complete: 0, 
    abort: 1, 
    revert: 2
}
ScriptFX.UI.AnimationStopState.createEnum('ScriptFX.UI.AnimationStopState', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Bounds

ScriptFX.UI.$create_Bounds = function ScriptFX_UI_Bounds(left, top, width, height) {
    var $o = { };
    $o.left = left;
    $o.top = top;
    $o.width = width;
    $o.height = height;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.DragDropData

ScriptFX.UI.$create_DragDropData = function ScriptFX_UI_DragDropData(mode, dataType, data) {
    var $o = { };
    $o.mode = mode;
    $o.dataType = dataType;
    $o.data = data;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.DragMode

ScriptFX.UI.DragMode = function() { };
ScriptFX.UI.DragMode.prototype = {
    move: 0, 
    copy: 1
}
ScriptFX.UI.DragMode.createEnum('ScriptFX.UI.DragMode', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IAction

ScriptFX.UI.IAction = function() { };
ScriptFX.UI.IAction.prototype = {
    get_actionArgument : null,
    get_actionName : null,
    add_action : null,
    remove_action : null
}
ScriptFX.UI.IAction.createInterface('ScriptFX.UI.IAction');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IDragDrop

ScriptFX.UI.IDragDrop = function() { };
ScriptFX.UI.IDragDrop.prototype = {
    get_supportsDataTransfer : null,
    dragDrop : null
}
ScriptFX.UI.IDragDrop.createInterface('ScriptFX.UI.IDragDrop');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IDragSource

ScriptFX.UI.IDragSource = function() { };
ScriptFX.UI.IDragSource.prototype = {
    get_domElement : null,
    onDragStart : null,
    onDrag : null,
    onDragEnd : null
}
ScriptFX.UI.IDragSource.createInterface('ScriptFX.UI.IDragSource');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IDropTarget

ScriptFX.UI.IDropTarget = function() { };
ScriptFX.UI.IDropTarget.prototype = {
    get_domElement : null,
    supportsDataObject : null,
    drop : null,
    onDragEnter : null,
    onDragLeave : null,
    onDragOver : null
}
ScriptFX.UI.IDropTarget.createInterface('ScriptFX.UI.IDropTarget');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IEditableText

ScriptFX.UI.IEditableText = function() { };
ScriptFX.UI.IEditableText.prototype = {
    get_text : null,
    set_text : null,
    add_textChanged : null,
    remove_textChanged : null
}
ScriptFX.UI.IEditableText.createInterface('ScriptFX.UI.IEditableText');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IStaticText

ScriptFX.UI.IStaticText = function() { };
ScriptFX.UI.IStaticText.prototype = {
    get_text : null
}
ScriptFX.UI.IStaticText.createInterface('ScriptFX.UI.IStaticText');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IToggle

ScriptFX.UI.IToggle = function() { };
ScriptFX.UI.IToggle.prototype = {
    get_checked : null,
    add_checkedChanged : null,
    remove_checkedChanged : null
}
ScriptFX.UI.IToggle.createInterface('ScriptFX.UI.IToggle');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.IValidator

ScriptFX.UI.IValidator = function() { };
ScriptFX.UI.IValidator.prototype = {
    get_isValid : null,
    get_validationGroup : null
}
ScriptFX.UI.IValidator.createInterface('ScriptFX.UI.IValidator');


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Key

ScriptFX.UI.Key = function() { };
ScriptFX.UI.Key.prototype = {
    backspace: 8, 
    tab: 9, 
    enter: 13, 
    escape: 27, 
    space: 32, 
    pageUp: 33, 
    pageDown: 34, 
    end: 35, 
    home: 36, 
    left: 37, 
    up: 38, 
    right: 39, 
    down: 40, 
    del: 127
}
ScriptFX.UI.Key.createEnum('ScriptFX.UI.Key', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Location

ScriptFX.UI.$create_Location = function ScriptFX_UI_Location(left, top) {
    var $o = { };
    $o.left = left;
    $o.top = top;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.OverlayOptions

ScriptFX.UI.$create_OverlayOptions = function ScriptFX_UI_OverlayOptions(cssClass) {
    var $o = { };
    $o.cssClass = cssClass;
    $o.fadeInOutInterval = 250;
    $o.opacity = 0.75;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.PopupMode

ScriptFX.UI.PopupMode = function() { };
ScriptFX.UI.PopupMode.prototype = {
    center: 0, 
    anchorTopLeft: 1, 
    anchorTopRight: 2, 
    anchorBottomRight: 3, 
    anchorBottomLeft: 4, 
    alignTopLeft: 5, 
    alignTopRight: 6, 
    alignBottomRight: 7, 
    alignBottomLeft: 8
}
ScriptFX.UI.PopupMode.createEnum('ScriptFX.UI.PopupMode', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.PopupOptions

ScriptFX.UI.$create_PopupOptions = function ScriptFX_UI_PopupOptions(referenceElement, mode) {
    var $o = { };
    $o.referenceElement = referenceElement;
    $o.mode = mode;
    $o.id = null;
    $o.xOffset = 0;
    $o.yOffset = 0;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Size

ScriptFX.UI.$create_Size = function ScriptFX_UI_Size(width, height) {
    var $o = { };
    $o.width = width;
    $o.height = height;
    return $o;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Animation

ScriptFX.UI.Animation = function ScriptFX_UI_Animation(domElement) {
    if (!domElement) {
        domElement = document.documentElement;
    }
    this._domElement = domElement;
    this._repeatCount = 1;
    ScriptFX.Application.current.registerDisposableObject(this);
}
ScriptFX.UI.Animation.prototype = {
    _domElement: null,
    _repeatCount: 0,
    _autoReverse: false,
    _repeatDelay: 0,
    _completed: false,
    _isPlaying: false,
    _isRepeating: false,
    _repetitions: 0,
    _repeatTimeStamp: 0,
    _reversed: false,
    
    add_repeating: function ScriptFX_UI_Animation$add_repeating(value) {
        this.__repeating = Delegate.combine(this.__repeating, value);
    },
    remove_repeating: function ScriptFX_UI_Animation$remove_repeating(value) {
        this.__repeating = Delegate.remove(this.__repeating, value);
    },
    
    __repeating: null,
    
    add_starting: function ScriptFX_UI_Animation$add_starting(value) {
        this.__starting = Delegate.combine(this.__starting, value);
    },
    remove_starting: function ScriptFX_UI_Animation$remove_starting(value) {
        this.__starting = Delegate.remove(this.__starting, value);
    },
    
    __starting: null,
    
    add_stopped: function ScriptFX_UI_Animation$add_stopped(value) {
        this.__stopped = Delegate.combine(this.__stopped, value);
    },
    remove_stopped: function ScriptFX_UI_Animation$remove_stopped(value) {
        this.__stopped = Delegate.remove(this.__stopped, value);
    },
    
    __stopped: null,
    
    get_autoReverse: function ScriptFX_UI_Animation$get_autoReverse() {
        return this._autoReverse;
    },
    set_autoReverse: function ScriptFX_UI_Animation$set_autoReverse(value) {
        Debug.assert(!this.get_isPlaying());
        this._autoReverse = value;
        return value;
    },
    
    get_completed: function ScriptFX_UI_Animation$get_completed() {
        return this._completed;
    },
    
    get_domElement: function ScriptFX_UI_Animation$get_domElement() {
        return this._domElement;
    },
    
    get_isPlaying: function ScriptFX_UI_Animation$get_isPlaying() {
        return this._isPlaying;
    },
    
    get_isReversed: function ScriptFX_UI_Animation$get_isReversed() {
        return this._reversed;
    },
    
    get_repeatCount: function ScriptFX_UI_Animation$get_repeatCount() {
        return this._repeatCount;
    },
    set_repeatCount: function ScriptFX_UI_Animation$set_repeatCount(value) {
        Debug.assert(!this.get_isPlaying());
        Debug.assert(value >= 0);
        this._repeatCount = value;
        return value;
    },
    
    get_repeatDelay: function ScriptFX_UI_Animation$get_repeatDelay() {
        return this._repeatDelay;
    },
    set_repeatDelay: function ScriptFX_UI_Animation$set_repeatDelay(value) {
        Debug.assert(!this.get_isPlaying());
        Debug.assert(value >= 0);
        this._repeatDelay = value;
        return value;
    },
    
    get_repetitions: function ScriptFX_UI_Animation$get_repetitions() {
        return this._repetitions;
    },
    
    dispose: function ScriptFX_UI_Animation$dispose() {
        if (this._isPlaying) {
            this.stop(ScriptFX.UI.AnimationStopState.abort);
        }
        if (this._domElement) {
            this._domElement = null;
            ScriptFX.Application.current.unregisterDisposableObject(this);
        }
    },
    
    _onPlay: function ScriptFX_UI_Animation$_onPlay(reversed) {
        if (this.__starting) {
            this.__starting.invoke(this, EventArgs.Empty);
        }
        this.performSetup();
        this._isPlaying = true;
        this._repetitions = 1;
        this._reversed = reversed;
        this.playCore();
    },
    
    _onStop: function ScriptFX_UI_Animation$_onStop(completed, stopState) {
        this.stopCore(completed, stopState);
        this._completed = completed;
        this._isPlaying = false;
        this.performCleanup();
        if (this.__stopped) {
            this.__stopped.invoke(this, EventArgs.Empty);
        }
    },
    
    _onProgress: function ScriptFX_UI_Animation$_onProgress(timeStamp) {
        if (this._isRepeating) {
            if ((this._repeatDelay) && ((this._repeatTimeStamp + this._repeatDelay) > timeStamp)) {
                return false;
            }
        }
        var completed = this.progressCore(this._isRepeating, timeStamp);
        this._isRepeating = false;
        if (completed && ((!this._repeatCount) || (this._repeatCount > this._repetitions))) {
            completed = false;
            this._repetitions++;
            if (this.__repeating) {
                var ce = new ScriptFX.CancelEventArgs();
                this.__repeating.invoke(this, ce);
                completed = ce.get_canceled();
            }
            if (!completed) {
                this._isRepeating = true;
                if (this._autoReverse) {
                    this._reversed = !this._reversed;
                }
                this._repeatTimeStamp = timeStamp;
                this.performRepetition(this._reversed);
            }
        }
        return completed;
    },
    
    performCleanup: function ScriptFX_UI_Animation$performCleanup() {
    },
    
    performRepetition: function ScriptFX_UI_Animation$performRepetition(reversed) {
    },
    
    performSetup: function ScriptFX_UI_Animation$performSetup() {
    },
    
    play: function ScriptFX_UI_Animation$play() {
        Debug.assert(!this.get_isPlaying());
        this._completed = false;
        ScriptFX.UI.AnimationManager._play(this);
    },
    
    stop: function ScriptFX_UI_Animation$stop(stopState) {
        Debug.assert(this.get_isPlaying());
        ScriptFX.UI.AnimationManager._stop(this, stopState);
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AnimationManager

ScriptFX.UI.AnimationManager = function ScriptFX_UI_AnimationManager() {
}
ScriptFX.UI.AnimationManager.get_FPS = function ScriptFX_UI_AnimationManager$get_FPS() {
    return ScriptFX.UI.AnimationManager._fps;
}
ScriptFX.UI.AnimationManager.set_FPS = function ScriptFX_UI_AnimationManager$set_FPS(value) {
    Debug.assert((value > 0) && (value <= 100));
    ScriptFX.UI.AnimationManager._fps = value;
    return value;
}
ScriptFX.UI.AnimationManager._onTick = function ScriptFX_UI_AnimationManager$_onTick() {
    ScriptFX.UI.AnimationManager._timerCookie = 0;
    if (!ScriptFX.UI.AnimationManager._activeAnimations.length) {
        return;
    }
    var timeStamp = (new Date()).getTime();
    var currentAnimations = ScriptFX.UI.AnimationManager._activeAnimations;
    var newAnimations = [];
    ScriptFX.UI.AnimationManager._activeAnimations = null;
    var $enum1 = currentAnimations.getEnumerator();
    while ($enum1.moveNext()) {
        var animation = $enum1.get_current();
        var completed = animation._onProgress(timeStamp);
        if (completed) {
            animation._onStop(true, ScriptFX.UI.AnimationStopState.complete);
        }
        else {
            newAnimations.add(animation);
        }
    }
    if (newAnimations.length) {
        ScriptFX.UI.AnimationManager._activeAnimations = newAnimations;
        if (!ScriptFX.UI.AnimationManager._timerCookie) {
            ScriptFX.UI.AnimationManager._timerCookie = window.setTimeout(Delegate.create(null, ScriptFX.UI.AnimationManager._onTick), 1000 / ScriptFX.UI.AnimationManager._fps);
        }
    }
}
ScriptFX.UI.AnimationManager._play = function ScriptFX_UI_AnimationManager$_play(animation) {
    if (!ScriptFX.UI.AnimationManager._activeAnimations) {
        ScriptFX.UI.AnimationManager._activeAnimations = [];
    }
    ScriptFX.UI.AnimationManager._activeAnimations.add(animation);
    animation._onPlay(false);
    if (!ScriptFX.UI.AnimationManager._timerCookie) {
        ScriptFX.UI.AnimationManager._timerCookie = window.setTimeout(Delegate.create(null, ScriptFX.UI.AnimationManager._onTick), 1000 / ScriptFX.UI.AnimationManager._fps);
    }
}
ScriptFX.UI.AnimationManager._stop = function ScriptFX_UI_AnimationManager$_stop(animation, stopState) {
    Debug.assert(ScriptFX.UI.AnimationManager._activeAnimations);
    animation._onStop(false, stopState);
    ScriptFX.UI.AnimationManager._activeAnimations.remove(animation);
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.AnimationSequence

ScriptFX.UI.AnimationSequence = function ScriptFX_UI_AnimationSequence(animations) {
    ScriptFX.UI.AnimationSequence.constructBase(this, [ null ]);
    Debug.assert((animations) && (animations.length > 1));
    this._animations$1 = animations;
    this._current$1 = -1;
}
ScriptFX.UI.AnimationSequence.prototype = {
    _animations$1: null,
    _successionDelay$1: 0,
    _current$1: 0,
    _nextAnimation$1: false,
    _successionTimeStamp$1: 0,
    
    get_successionDelay: function ScriptFX_UI_AnimationSequence$get_successionDelay() {
        return this._successionDelay$1;
    },
    set_successionDelay: function ScriptFX_UI_AnimationSequence$set_successionDelay(value) {
        Debug.assert(!this.get_isPlaying());
        Debug.assert(value >= 0);
        this._successionDelay$1 = value;
        return value;
    },
    
    playCore: function ScriptFX_UI_AnimationSequence$playCore() {
        Debug.assert(this._current$1 === -1);
        if (!this.get_isReversed()) {
            this._current$1 = 0;
        }
        else {
            this._current$1 = this._animations$1.length - 1;
        }
        this._animations$1[this._current$1]._onPlay(this.get_isReversed());
    },
    
    progressCore: function ScriptFX_UI_AnimationSequence$progressCore(startRepetition, timeStamp) {
        if (startRepetition) {
            if (!this.get_isReversed()) {
                this._current$1 = 0;
            }
            else {
                this._current$1 = this._animations$1.length - 1;
            }
            this._nextAnimation$1 = true;
        }
        var animation = this._animations$1[this._current$1];
        if (this._nextAnimation$1) {
            if ((this._successionDelay$1) && ((this._successionTimeStamp$1 + this._successionDelay$1) > timeStamp)) {
                return false;
            }
            this._nextAnimation$1 = false;
            animation._onPlay(this.get_isReversed());
        }
        var completed = animation._onProgress(timeStamp);
        if (completed) {
            animation._onStop(true, ScriptFX.UI.AnimationStopState.complete);
            if (!this.get_isReversed()) {
                this._current$1++;
            }
            else {
                this._current$1--;
            }
            this._nextAnimation$1 = true;
            this._successionTimeStamp$1 = timeStamp;
        }
        return completed && ((this._current$1 === this._animations$1.length) || (this._current$1 === -1));
    },
    
    stopCore: function ScriptFX_UI_AnimationSequence$stopCore(completed, stopState) {
        if (!completed) {
            var animation = this._animations$1[this._current$1];
            animation._onStop(false, stopState);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Behavior

ScriptFX.UI.Behavior = function ScriptFX_UI_Behavior(domElement, id) {
    Debug.assert(domElement);
    ScriptFX.Application.current.registerDisposableObject(this);
    this._domElement = domElement;
    this._id = id;
    if (!String.isNullOrEmpty(id)) {
        if (id === 'control') {
            var existingControl = domElement[id];
            if ((existingControl) && (Type.getInstanceType(existingControl) === ScriptFX.UI._genericControl)) {
                delete domElement.control;
                ScriptFX.Application.current.unregisterDisposableObject(existingControl);
                this._events = existingControl.get__eventsInternal();
            }
        }
        Debug.assert(isUndefined(domElement[id]));
        domElement[id] = this;
    }
    if (id !== 'control') {
        var existingControl = domElement.control;
        if (!existingControl) {
            existingControl = new ScriptFX.UI._genericControl(domElement);
        }
    }
    var behaviors = domElement._behaviors;
    if (!behaviors) {
        behaviors = [];
        domElement._behaviors = behaviors;
    }
    behaviors.add(this);
}
ScriptFX.UI.Behavior.getBehavior = function ScriptFX_UI_Behavior$getBehavior(domElement, type) {
    Debug.assert(domElement);
    Debug.assert(type);
    var behaviors = domElement._behaviors;
    if (behaviors) {
        var $enum1 = behaviors.getEnumerator();
        while ($enum1.moveNext()) {
            var behavior = $enum1.get_current();
            if (type.isAssignableFrom(Type.getInstanceType(behavior))) {
                return behavior;
            }
        }
    }
    return null;
}
ScriptFX.UI.Behavior.getBehaviors = function ScriptFX_UI_Behavior$getBehaviors(domElement, type) {
    Debug.assert(domElement);
    var behaviors = domElement._behaviors;
    if (isNullOrUndefined(behaviors) || (!behaviors.length)) {
        return null;
    }
    if (!type) {
        return behaviors.clone();
    }
    return behaviors.filter(Delegate.create(null, function(behavior) {
        return type.isAssignableFrom(Type.getInstanceType(behavior));
    }));
}
ScriptFX.UI.Behavior.getNamedBehavior = function ScriptFX_UI_Behavior$getNamedBehavior(domElement, id) {
    Debug.assert(domElement);
    Debug.assert(!String.isNullOrEmpty(id));
    return domElement[id];
}
ScriptFX.UI.Behavior.prototype = {
    _domElement: null,
    _id: null,
    _domEvents: null,
    _events: null,
    _initializing: false,
    
    get_domElement: function ScriptFX_UI_Behavior$get_domElement() {
        return this._domElement;
    },
    
    get_domEvents: function ScriptFX_UI_Behavior$get_domEvents() {
        if (!this._domEvents) {
            this._domEvents = new ScriptFX.UI.DOMEventList(this._domElement);
        }
        return this._domEvents;
    },
    
    get_events: function ScriptFX_UI_Behavior$get_events() {
        if (!this._events) {
            this._events = new ScriptFX.EventList();
        }
        return this._events;
    },
    
    get__eventsInternal: function ScriptFX_UI_Behavior$get__eventsInternal() {
        return this._events;
    },
    
    get_isDisposed: function ScriptFX_UI_Behavior$get_isDisposed() {
        return (!this._domElement);
    },
    
    get_isInitializing: function ScriptFX_UI_Behavior$get_isInitializing() {
        return this._initializing;
    },
    
    add_propertyChanged: function ScriptFX_UI_Behavior$add_propertyChanged(value) {
        this.get_events().addHandler('PropertyChanged', value);
    },
    remove_propertyChanged: function ScriptFX_UI_Behavior$remove_propertyChanged(value) {
        this.get_events().removeHandler('PropertyChanged', value);
    },
    
    beginInitialize: function ScriptFX_UI_Behavior$beginInitialize() {
        this._initializing = true;
    },
    
    dispose: function ScriptFX_UI_Behavior$dispose() {
        if (this._domEvents) {
            this._domEvents.dispose();
        }
        if (this._domElement) {
            if (this._id) {
                if (ScriptFX.Application.current.get_isIE()) {
                    this._domElement.removeAttribute(this._id);
                }
                else {
                    delete this._domElement[this._id];
                }
            }
            var behaviors = this._domElement._behaviors;
            Debug.assert(behaviors);
            behaviors.remove(this);
            this._domElement = null;
            ScriptFX.Application.current.unregisterDisposableObject(this);
        }
    },
    
    endInitialize: function ScriptFX_UI_Behavior$endInitialize() {
        this._initializing = false;
    },
    
    raisePropertyChanged: function ScriptFX_UI_Behavior$raisePropertyChanged(propertyName) {
        var propChangedHandler = this.get_events().getHandler('PropertyChanged');
        if (propChangedHandler) {
            propChangedHandler.invoke(this, new ScriptFX.PropertyChangedEventArgs(propertyName));
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Color

ScriptFX.UI.Color = function ScriptFX_UI_Color(red, green, blue) {
    Debug.assert(red >= 0 && red <= 255);
    Debug.assert(green >= 0 && green <= 255);
    Debug.assert(blue >= 0 && blue <= 255);
    this._red = red;
    this._green = green;
    this._blue = blue;
}
ScriptFX.UI.Color.format = function ScriptFX_UI_Color$format(red, green, blue) {
    return String.format('#{0:X2}{1:X2}{2:X2}', red, green, blue);
}
ScriptFX.UI.Color.parse = function ScriptFX_UI_Color$parse(s) {
    if (String.isNullOrEmpty(s)) {
        return null;
    }
    if ((s.length === 7) && s.startsWith('#')) {
        var red = parseInt(s.substr(1, 2), 16);
        var green = parseInt(s.substr(3, 2), 16);
        var blue = parseInt(s.substr(5, 2), 16);
        return new ScriptFX.UI.Color(red, green, blue);
    }
    else if (s.startsWith('rgb(') && s.endsWith(')')) {
        var parts = s.substring(4, s.length - 1).split(',');
        if (parts.length === 3) {
            return new ScriptFX.UI.Color(parseInt(parts[0].trim()), parseInt(parts[1].trim()), parseInt(parts[2].trim()));
        }
    }
    return null;
}
ScriptFX.UI.Color.prototype = {
    _red: 0,
    _green: 0,
    _blue: 0,
    
    get_blue: function ScriptFX_UI_Color$get_blue() {
        return this._blue;
    },
    
    get_green: function ScriptFX_UI_Color$get_green() {
        return this._green;
    },
    
    get_red: function ScriptFX_UI_Color$get_red() {
        return this._red;
    },
    
    toString: function ScriptFX_UI_Color$toString() {
        return ScriptFX.UI.Color.format(this._red, this._green, this._blue);
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Control

ScriptFX.UI.Control = function ScriptFX_UI_Control(domElement) {
    ScriptFX.UI.Control.constructBase(this, [ domElement, 'control' ]);
}
ScriptFX.UI.Control.getControl = function ScriptFX_UI_Control$getControl(domElement) {
    return ScriptFX.UI.Behavior.getNamedBehavior(domElement, 'control');
}
ScriptFX.UI.Control.prototype = {
    
    add_disposing: function ScriptFX_UI_Control$add_disposing(value) {
        this.get_events().addHandler('disposing', value);
    },
    remove_disposing: function ScriptFX_UI_Control$remove_disposing(value) {
        this.get_events().removeHandler('disposing', value);
    },
    
    dispose: function ScriptFX_UI_Control$dispose() {
        var element = this.get_domElement();
        if (element) {
            var disposingHandler = this.get_events().getHandler('disposing');
            if (disposingHandler) {
                disposingHandler.invoke(this, EventArgs.Empty);
            }
            var behaviors = ScriptFX.UI.Behavior.getBehaviors(element, null);
            Debug.assert((behaviors) && (behaviors.length > 0));
            if (behaviors.length > 1) {
                var $enum1 = behaviors.getEnumerator();
                while ($enum1.moveNext()) {
                    var behavior = $enum1.get_current();
                    if (behavior !== this) {
                        behavior.dispose();
                    }
                }
            }
        }
        ScriptFX.UI.Control.callBase(this, 'dispose');
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.DOMEventList

ScriptFX.UI.DOMEventList = function ScriptFX_UI_DOMEventList(element) {
    Debug.assert(element);
    this._element = element;
    this._handlers = {};
}
ScriptFX.UI.DOMEventList.prototype = {
    _element: null,
    _handlers: null,
    
    attach: function ScriptFX_UI_DOMEventList$attach(eventName, handler) {
        Debug.assert(this._element);
        Debug.assert(!String.isNullOrEmpty(eventName));
        Debug.assert(handler);
        Debug.assert(!this.isAttached(eventName));
        this._element.attachEvent(eventName, handler);
        this._handlers[eventName] = handler;
    },
    
    detach: function ScriptFX_UI_DOMEventList$detach(eventName) {
        Debug.assert(this._element);
        Debug.assert(!String.isNullOrEmpty(eventName));
        var handler = this._handlers[eventName];
        if (handler) {
            this._element.detachEvent(eventName, handler);
            delete this._handlers[eventName];
            return true;
        }
        return false;
    },
    
    dispose: function ScriptFX_UI_DOMEventList$dispose() {
        if (this._element) {
            var $dict1 = this._handlers;
            for (var $key2 in $dict1) {
                var e = { key: $key2, value: $dict1[$key2] };
                this._element.detachEvent(e.key, e.value);
            }
            this._element = null;
            this._handlers = null;
        }
    },
    
    isAttached: function ScriptFX_UI_DOMEventList$isAttached(eventName) {
        Debug.assert(this._element);
        Debug.assert(!String.isNullOrEmpty(eventName));
        return (this._handlers[eventName]) ? true : false;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.DragDropEventArgs

ScriptFX.UI.DragDropEventArgs = function ScriptFX_UI_DragDropEventArgs(dataObject) {
    ScriptFX.UI.DragDropEventArgs.constructBase(this);
    this._dataObject$1 = dataObject;
}
ScriptFX.UI.DragDropEventArgs.prototype = {
    _dataObject$1: null,
    
    get_dataObject: function ScriptFX_UI_DragDropEventArgs$get_dataObject() {
        return this._dataObject$1;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.DragDropManager

ScriptFX.UI.DragDropManager = function ScriptFX_UI_DragDropManager() {
}
ScriptFX.UI.DragDropManager.get_canDragDrop = function ScriptFX_UI_DragDropManager$get_canDragDrop() {
    return (ScriptFX.UI.DragDropManager._dragDropImplementation);
}
ScriptFX.UI.DragDropManager.get_supportsDataTransfer = function ScriptFX_UI_DragDropManager$get_supportsDataTransfer() {
    Debug.assert(ScriptFX.UI.DragDropManager.get_canDragDrop());
    return ScriptFX.UI.DragDropManager._dragDropImplementation.get_supportsDataTransfer();
}
ScriptFX.UI.DragDropManager.add_dragDropEnding = function ScriptFX_UI_DragDropManager$add_dragDropEnding(value) {
    ScriptFX.UI.DragDropManager._dragEndingHandler = Delegate.combine(ScriptFX.UI.DragDropManager._dragEndingHandler, value);
}
ScriptFX.UI.DragDropManager.remove_dragDropEnding = function ScriptFX_UI_DragDropManager$remove_dragDropEnding(value) {
    ScriptFX.UI.DragDropManager._dragEndingHandler = Delegate.remove(ScriptFX.UI.DragDropManager._dragEndingHandler, value);
}
ScriptFX.UI.DragDropManager.add_dragDropStarting = function ScriptFX_UI_DragDropManager$add_dragDropStarting(value) {
    ScriptFX.UI.DragDropManager._dragStartingHandler = Delegate.combine(ScriptFX.UI.DragDropManager._dragStartingHandler, value);
}
ScriptFX.UI.DragDropManager.remove_dragDropStarting = function ScriptFX_UI_DragDropManager$remove_dragDropStarting(value) {
    ScriptFX.UI.DragDropManager._dragStartingHandler = Delegate.remove(ScriptFX.UI.DragDropManager._dragStartingHandler, value);
}
ScriptFX.UI.DragDropManager._endDragDrop = function ScriptFX_UI_DragDropManager$_endDragDrop() {
    if (ScriptFX.UI.DragDropManager._dragEndingHandler) {
        ScriptFX.UI.DragDropManager._dragEndingHandler.invoke(null, new ScriptFX.UI.DragDropEventArgs(ScriptFX.UI.DragDropManager._currentDataObject));
    }
    ScriptFX.UI.DragDropManager._currentDataObject = null;
}
ScriptFX.UI.DragDropManager.registerDragDropImplementation = function ScriptFX_UI_DragDropManager$registerDragDropImplementation(dragDrop) {
    ScriptFX.UI.DragDropManager._dragDropImplementation = dragDrop;
}
ScriptFX.UI.DragDropManager.registerDropTarget = function ScriptFX_UI_DragDropManager$registerDropTarget(target) {
    ScriptFX.UI.DragDropManager._dropTargets.add(target);
}
ScriptFX.UI.DragDropManager.startDragDrop = function ScriptFX_UI_DragDropManager$startDragDrop(data, dragVisual, dragOffset, source, context) {
    Debug.assert(ScriptFX.UI.DragDropManager.get_canDragDrop());
    if (ScriptFX.UI.DragDropManager._currentDataObject) {
        return false;
    }
    var validDropTargets = [];
    var $enum1 = ScriptFX.UI.DragDropManager._dropTargets.getEnumerator();
    while ($enum1.moveNext()) {
        var dropTarget = $enum1.get_current();
        if (dropTarget.supportsDataObject(data)) {
            validDropTargets.add(dropTarget);
        }
    }
    if (!validDropTargets.length) {
        return false;
    }
    ScriptFX.UI.DragDropManager._currentDataObject = data;
    if (ScriptFX.UI.DragDropManager._dragStartingHandler) {
        ScriptFX.UI.DragDropManager._dragStartingHandler.invoke(null, new ScriptFX.UI.DragDropEventArgs(data));
    }
    ScriptFX.UI.DragDropManager._dragDropImplementation.dragDrop(new ScriptFX.UI._dragDropTracker(source), context, validDropTargets, dragVisual, dragOffset, ScriptFX.UI.DragDropManager._currentDataObject);
    return true;
}
ScriptFX.UI.DragDropManager.unregisterDropTarget = function ScriptFX_UI_DragDropManager$unregisterDropTarget(target) {
    ScriptFX.UI.DragDropManager._dropTargets.remove(target);
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI._dragDropTracker

ScriptFX.UI._dragDropTracker = function ScriptFX_UI__dragDropTracker(actualSource) {
    this._actualSource = actualSource;
}
ScriptFX.UI._dragDropTracker.prototype = {
    _actualSource: null,
    
    get_domElement: function ScriptFX_UI__dragDropTracker$get_domElement() {
        return this._actualSource.get_domElement();
    },
    
    onDragStart: function ScriptFX_UI__dragDropTracker$onDragStart(context) {
        if (this._actualSource) {
            this._actualSource.onDragStart(context);
        }
    },
    
    onDrag: function ScriptFX_UI__dragDropTracker$onDrag(context) {
        if (this._actualSource) {
            this._actualSource.onDrag(context);
        }
    },
    
    onDragEnd: function ScriptFX_UI__dragDropTracker$onDragEnd(canceled, context) {
        if (this._actualSource) {
            this._actualSource.onDragEnd(canceled, context);
        }
        ScriptFX.UI.DragDropManager._endDragDrop();
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.Element

ScriptFX.UI.Element = function ScriptFX_UI_Element() {
}
ScriptFX.UI.Element.addCSSClass = function ScriptFX_UI_Element$addCSSClass(element, className) {
    var cssClass = element.className;
    if (cssClass.indexOf(className) < 0) {
        element.className = cssClass + ' ' + className;
    }
}
ScriptFX.UI.Element.containsCSSClass = function ScriptFX_UI_Element$containsCSSClass(element, className) {
    return element.className.split(' ').contains(className);
}
ScriptFX.UI.Element.getBounds = function ScriptFX_UI_Element$getBounds(element) {
    var location = ScriptFX.UI.Element.getLocation(element);
    return ScriptFX.UI.$create_Bounds(location.left, location.top, element.offsetWidth, element.offsetHeight);
}
ScriptFX.UI.Element.getLocation = function ScriptFX_UI_Element$getLocation(element) {
    var offsetX = 0;
    var offsetY = 0;
    for (var parentElement = element; parentElement; parentElement = parentElement.offsetParent) {
        offsetX += parentElement.offsetLeft;
        offsetY += parentElement.offsetTop;
    }
    return ScriptFX.UI.$create_Location(offsetX, offsetY);
}
ScriptFX.UI.Element.getSize = function ScriptFX_UI_Element$getSize(element) {
    return ScriptFX.UI.$create_Size(element.offsetWidth, element.offsetHeight);
}
ScriptFX.UI.Element.removeCSSClass = function ScriptFX_UI_Element$removeCSSClass(element, className) {
    var cssClass = ' ' + element.className + ' ';
    var index = cssClass.indexOf(' ' + className + ' ');
    if (index >= 0) {
        var newClass = cssClass.substr(0, index) + ' ' + cssClass.substr(index + className.length + 1);
        element.className = newClass;
    }
}
ScriptFX.UI.Element.setLocation = function ScriptFX_UI_Element$setLocation(element, location) {
    element.style.left = location.left + 'px';
    element.style.top = location.top + 'px';
}
ScriptFX.UI.Element.setSize = function ScriptFX_UI_Element$setSize(element, size) {
    element.style.width = size.width + 'px';
    element.style.height = size.height + 'px';
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.FadeEffect

ScriptFX.UI.FadeEffect = function ScriptFX_UI_FadeEffect(domElement, duration, opacity) {
    ScriptFX.UI.FadeEffect.constructBase(this, [ domElement, duration ]);
    this._opacity$2 = opacity;
}
ScriptFX.UI.FadeEffect.prototype = {
    _fadingIn$2: false,
    _opacity$2: 0,
    
    get_isFadingIn: function ScriptFX_UI_FadeEffect$get_isFadingIn() {
        return this._fadingIn$2;
    },
    
    fadeIn: function ScriptFX_UI_FadeEffect$fadeIn() {
        if (this.get_isPlaying()) {
            this.stop(ScriptFX.UI.AnimationStopState.complete);
        }
        this._fadingIn$2 = true;
        this.play();
    },
    
    fadeOut: function ScriptFX_UI_FadeEffect$fadeOut() {
        if (this.get_isPlaying()) {
            this.stop(ScriptFX.UI.AnimationStopState.complete);
        }
        this._fadingIn$2 = false;
        this.play();
    },
    
    performCleanup: function ScriptFX_UI_FadeEffect$performCleanup() {
        ScriptFX.UI.FadeEffect.callBase(this, 'performCleanup');
        if (!this._fadingIn$2) {
            this._setOpacity$2(0);
            this.get_domElement().style.display = 'none';
        }
    },
    
    performSetup: function ScriptFX_UI_FadeEffect$performSetup() {
        ScriptFX.UI.FadeEffect.callBase(this, 'performSetup');
        if (this._fadingIn$2) {
            this._setOpacity$2(0);
            this.get_domElement().style.display = '';
        }
    },
    
    performTweening: function ScriptFX_UI_FadeEffect$performTweening(frame) {
        if (this._fadingIn$2) {
            this._setOpacity$2(this._opacity$2 * frame);
        }
        else {
            this._setOpacity$2(this._opacity$2 * (1 - frame));
        }
    },
    
    _setOpacity$2: function ScriptFX_UI_FadeEffect$_setOpacity$2(opacity) {
        if (ScriptFX.Application.current.get_isIE()) {
            this.get_domElement().style.filter = 'alpha(opacity=' + (opacity * 100) + ')';
        }
        else {
            this.get_domElement().style.opacity = opacity.toString();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI._genericControl

ScriptFX.UI._genericControl = function ScriptFX_UI__genericControl(domElement) {
    ScriptFX.UI._genericControl.constructBase(this, [ domElement ]);
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.OverlayBehavior

ScriptFX.UI.OverlayBehavior = function ScriptFX_UI_OverlayBehavior(domElement, options) {
    ScriptFX.UI.OverlayBehavior.constructBase(this, [ domElement, options.id ]);
    this._overlayElement$1 = document.createElement('div');
    this._overlayElement$1.className = options.cssClass;
    var overlayStyle = this._overlayElement$1.style;
    overlayStyle.display = 'none';
    overlayStyle.top = '0px';
    overlayStyle.left = '0px';
    overlayStyle.width = '100%';
    if (ScriptFX.Application.current.get_isIE() && (ScriptFX.Application.current.get_host().get_majorVersion() < 7)) {
        overlayStyle.position = 'absolute';
    }
    else {
        this._fixedOverlayElement$1 = true;
        overlayStyle.position = 'fixed';
        overlayStyle.height = '100%';
    }
    document.body.appendChild(this._overlayElement$1);
    if (options.fadeInOutInterval) {
        this._fade$1 = new ScriptFX.UI.FadeEffect(this._overlayElement$1, options.fadeInOutInterval, options.opacity);
        this._fade$1.set_easingFunction(Delegate.create(null, ScriptFX.UI.TimedAnimation.easeInOut));
        this._fade$1.add_stopped(Delegate.create(this, this._onAnimationStopped$1));
    }
}
ScriptFX.UI.OverlayBehavior.prototype = {
    _overlayElement$1: null,
    _fixedOverlayElement$1: false,
    _fade$1: null,
    _resizeHandler$1: null,
    _visible$1: false,
    
    get_isVisible: function ScriptFX_UI_OverlayBehavior$get_isVisible() {
        return this._visible$1;
    },
    
    add_visibilityChanged: function ScriptFX_UI_OverlayBehavior$add_visibilityChanged(value) {
        this.get_events().addHandler(ScriptFX.UI.OverlayBehavior._visibilityChangedEventKey$1, value);
    },
    remove_visibilityChanged: function ScriptFX_UI_OverlayBehavior$remove_visibilityChanged(value) {
        this.get_events().removeHandler(ScriptFX.UI.OverlayBehavior._visibilityChangedEventKey$1, value);
    },
    
    dispose: function ScriptFX_UI_OverlayBehavior$dispose() {
        if (this._fade$1) {
            this._fade$1.dispose();
            this._fade$1 = null;
        }
        if (this._resizeHandler$1) {
            window.detachEvent('onresize', this._resizeHandler$1);
            this._resizeHandler$1 = null;
        }
        ScriptFX.UI.OverlayBehavior.callBase(this, 'dispose');
    },
    
    hide: function ScriptFX_UI_OverlayBehavior$hide() {
        if ((!this._visible$1) || this._fade$1.get_isPlaying()) {
            return;
        }
        if (this._resizeHandler$1) {
            window.detachEvent('onresize', this._resizeHandler$1);
            this._resizeHandler$1 = null;
        }
        if (this._fade$1) {
            this._fade$1.fadeOut();
        }
        else {
            this._overlayElement$1.style.display = 'none';
            this._visible$1 = false;
            var handler = this.get_events().getHandler(ScriptFX.UI.OverlayBehavior._visibilityChangedEventKey$1);
            if (handler) {
                handler.invoke(this, EventArgs.Empty);
            }
        }
    },
    
    _onAnimationStopped$1: function ScriptFX_UI_OverlayBehavior$_onAnimationStopped$1(sender, e) {
        this._visible$1 = this._fade$1.get_isFadingIn();
        var handler = this.get_events().getHandler(ScriptFX.UI.OverlayBehavior._visibilityChangedEventKey$1);
        if (handler) {
            handler.invoke(this, EventArgs.Empty);
        }
    },
    
    _onWindowResize$1: function ScriptFX_UI_OverlayBehavior$_onWindowResize$1() {
        this._overlayElement$1.style.height = document.documentElement.offsetHeight + 'px';
    },
    
    show: function ScriptFX_UI_OverlayBehavior$show() {
        if (this._visible$1 || this._fade$1.get_isPlaying()) {
            return;
        }
        if (!this._fixedOverlayElement$1) {
            this._overlayElement$1.style.height = document.documentElement.offsetHeight + 'px';
            this._resizeHandler$1 = Delegate.create(this, this._onWindowResize$1);
            window.attachEvent('onresize', this._resizeHandler$1);
        }
        if (this._fade$1) {
            this._fade$1.fadeIn();
        }
        else {
            this._overlayElement$1.style.display = '';
            this._visible$1 = true;
            var handler = this.get_events().getHandler(ScriptFX.UI.OverlayBehavior._visibilityChangedEventKey$1);
            if (handler) {
                handler.invoke(this, EventArgs.Empty);
            }
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.PopupBehavior

ScriptFX.UI.PopupBehavior = function ScriptFX_UI_PopupBehavior(domElement, options) {
    ScriptFX.UI.PopupBehavior.constructBase(this, [ domElement, options.id ]);
    this._options$1 = options;
    domElement.style.position = 'absolute';
    domElement.style.display = 'none';
}
ScriptFX.UI.PopupBehavior.prototype = {
    _options$1: null,
    _iframe$1: null,
    
    dispose: function ScriptFX_UI_PopupBehavior$dispose() {
        if (this.get_domElement()) {
            this.hide();
        }
        ScriptFX.UI.PopupBehavior.callBase(this, 'dispose');
    },
    
    hide: function ScriptFX_UI_PopupBehavior$hide() {
        this.get_domElement().style.display = 'none';
        if (this._iframe$1) {
            this._iframe$1.parentNode.removeChild(this._iframe$1);
            this._iframe$1 = null;
        }
    },
    
    show: function ScriptFX_UI_PopupBehavior$show() {
        var parentElement = this.get_domElement().offsetParent;
        if (!parentElement) {
            parentElement = document.documentElement;
        }
        this.get_domElement().style.display = 'block';
        var x = 0;
        var y = 0;
        var xOffsetDirection = 1;
        var yOffsetDirection = 1;
        var alignment = false;
        var parentBounds = ScriptFX.UI.Element.getBounds(parentElement);
        var elementBounds = ScriptFX.UI.Element.getBounds(this.get_domElement());
        var referenceBounds = ScriptFX.UI.Element.getBounds(this._options$1.referenceElement);
        var xDelta = referenceBounds.left - parentBounds.left;
        var yDelta = referenceBounds.top - parentBounds.top;
        switch (this._options$1.mode) {
            case ScriptFX.UI.PopupMode.center:
                x = Math.round(referenceBounds.width / 2 - elementBounds.width / 2);
                y = Math.round(referenceBounds.height / 2 - elementBounds.height / 2);
                break;
            case ScriptFX.UI.PopupMode.anchorTopLeft:
                x = 0;
                y = -elementBounds.height;
                break;
            case ScriptFX.UI.PopupMode.anchorTopRight:
                x = referenceBounds.width - elementBounds.width;
                y = -elementBounds.height;
                break;
            case ScriptFX.UI.PopupMode.anchorBottomRight:
                x = referenceBounds.width - elementBounds.width;
                y = referenceBounds.height;
                break;
            case ScriptFX.UI.PopupMode.anchorBottomLeft:
                x = 0;
                y = referenceBounds.height;
                break;
            case ScriptFX.UI.PopupMode.alignTopLeft:
                x = referenceBounds.left;
                y = referenceBounds.top;
                alignment = true;
                break;
            case ScriptFX.UI.PopupMode.alignTopRight:
                x = referenceBounds.left + referenceBounds.width - elementBounds.width;
                y = referenceBounds.top;
                xOffsetDirection = -1;
                alignment = true;
                break;
            case ScriptFX.UI.PopupMode.alignBottomRight:
                x = referenceBounds.left + referenceBounds.width - elementBounds.width;
                y = referenceBounds.top + referenceBounds.height - elementBounds.height;
                xOffsetDirection = -1;
                yOffsetDirection = -1;
                alignment = true;
                break;
            case ScriptFX.UI.PopupMode.alignBottomLeft:
                x = referenceBounds.left;
                y = referenceBounds.top + referenceBounds.height - elementBounds.height;
                yOffsetDirection = -1;
                alignment = true;
                break;
        }
        if (!alignment) {
            x += xDelta + this._options$1.xOffset;
            y += yDelta + this._options$1.yOffset;
        }
        else {
            x += xDelta + this._options$1.xOffset * xOffsetDirection;
            y += yDelta + this._options$1.yOffset * yOffsetDirection;
        }
        var docWidth = document.body.clientWidth;
        if (x + elementBounds.width > docWidth - 2) {
            x -= (x + elementBounds.width - docWidth + 2);
        }
        if (x < 0) {
            x = 2;
        }
        if (y < 0) {
            y = 2;
        }
        ScriptFX.UI.Element.setLocation(this.get_domElement(), ScriptFX.UI.$create_Location(x, y));
        var host = ScriptFX.Application.current.get_host();
        if ((host.get_name() === ScriptFX.HostName.IE) && (host.get_majorVersion() < 7)) {
            this._iframe$1 = document.createElement('IFRAME');
            this._iframe$1.src = 'javascript:false;';
            this._iframe$1.scrolling = 'no';
            this._iframe$1.style.position = 'absolute';
            this._iframe$1.style.display = 'block';
            this._iframe$1.style.border = 'none';
            this._iframe$1.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)';
            this._iframe$1.style.left = x + 'px';
            this._iframe$1.style.top = y + 'px';
            this._iframe$1.style.width = elementBounds.width + 'px';
            this._iframe$1.style.height = elementBounds.height + 'px';
            this._iframe$1.style.zIndex = 1;
            this.get_domElement().parentNode.insertBefore(this._iframe$1, this.get_domElement());
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.UI.TimedAnimation

ScriptFX.UI.TimedAnimation = function ScriptFX_UI_TimedAnimation(domElement, duration) {
    ScriptFX.UI.TimedAnimation.constructBase(this, [ domElement ]);
    Debug.assert(duration > 0);
    this._duration$1 = duration;
}
ScriptFX.UI.TimedAnimation.easeIn = function ScriptFX_UI_TimedAnimation$easeIn(t) {
    return t * t;
}
ScriptFX.UI.TimedAnimation.easeInOut = function ScriptFX_UI_TimedAnimation$easeInOut(t) {
    t = t * 2;
    if (t < 1) {
        return t * t / 2;
    }
    return -((--t) * (t - 2) - 1) / 2;
}
ScriptFX.UI.TimedAnimation.easeOut = function ScriptFX_UI_TimedAnimation$easeOut(t) {
    return -t * (t - 2);
}
ScriptFX.UI.TimedAnimation.prototype = {
    _duration$1: 0,
    _easingFunction$1: null,
    _startTimeStamp$1: 0,
    
    get_duration: function ScriptFX_UI_TimedAnimation$get_duration() {
        return this._duration$1;
    },
    set_duration: function ScriptFX_UI_TimedAnimation$set_duration(value) {
        Debug.assert(!this.get_isPlaying());
        Debug.assert(this._duration$1 >= 0);
        this._duration$1 = value;
        return value;
    },
    
    get_easingFunction: function ScriptFX_UI_TimedAnimation$get_easingFunction() {
        return this._easingFunction$1;
    },
    set_easingFunction: function ScriptFX_UI_TimedAnimation$set_easingFunction(value) {
        Debug.assert(!this.get_isPlaying());
        this._easingFunction$1 = value;
        return value;
    },
    
    playCore: function ScriptFX_UI_TimedAnimation$playCore() {
        this._startTimeStamp$1 = (new Date()).getTime();
        this.progressCore(false, this._startTimeStamp$1);
    },
    
    progressCore: function ScriptFX_UI_TimedAnimation$progressCore(startRepetition, timeStamp) {
        var frame = 0;
        var completed = false;
        if (!startRepetition) {
            frame = (timeStamp - this._startTimeStamp$1) / this._duration$1;
            if (!this.get_isReversed()) {
                completed = (frame >= 1);
                frame = Math.min(1, frame);
            }
            else {
                frame = 1 - frame;
                completed = (frame <= 0);
                frame = Math.max(0, frame);
            }
            if ((!completed) && (this._easingFunction$1)) {
                frame = this._easingFunction$1.invoke(frame);
            }
        }
        else {
            this._startTimeStamp$1 = timeStamp;
            if (this.get_isReversed()) {
                frame = 1;
            }
        }
        this.performTweening(frame);
        return completed;
    },
    
    stopCore: function ScriptFX_UI_TimedAnimation$stopCore(completed, stopState) {
        if (!completed) {
            if (stopState === ScriptFX.UI.AnimationStopState.complete) {
                this.performTweening(1);
            }
            else if (stopState === ScriptFX.UI.AnimationStopState.revert) {
                this.performTweening(0);
            }
        }
    }
}


ScriptFX.Application.createClass('ScriptFX.Application', null, IServiceProvider, IServiceContainer, ScriptFX.IEventManager);
ScriptFX.CancelEventArgs.createClass('ScriptFX.CancelEventArgs', EventArgs);
ScriptFX.CollectionChangedEventArgs.createClass('ScriptFX.CollectionChangedEventArgs', EventArgs);
ScriptFX.ApplicationUnloadingEventArgs.createClass('ScriptFX.ApplicationUnloadingEventArgs', EventArgs);
ScriptFX.HistoryManager.createClass('ScriptFX.HistoryManager', null, IDisposable);
ScriptFX.HistoryEventArgs.createClass('ScriptFX.HistoryEventArgs', EventArgs);
ScriptFX.HostInfo.createClass('ScriptFX.HostInfo');
ScriptFX.EventList.createClass('ScriptFX.EventList');
ScriptFX.JSON.createClass('ScriptFX.JSON');
ScriptFX.PropertyChangedEventArgs.createClass('ScriptFX.PropertyChangedEventArgs', EventArgs);
ScriptFX.ObservableCollection.createClass('ScriptFX.ObservableCollection', null, IDisposable, IArray, IEnumerable, ScriptFX.INotifyCollectionChanged);
ScriptFX.Net.HTTPRequest.createClass('ScriptFX.Net.HTTPRequest', null, IDisposable);
ScriptFX.Net.HTTPRequestManager.createClass('ScriptFX.Net.HTTPRequestManager');
ScriptFX.Net.HTTPTransport.createClass('ScriptFX.Net.HTTPTransport', null, IDisposable);
ScriptFX.Net.PostHTTPRequestEventArgs.createClass('ScriptFX.Net.PostHTTPRequestEventArgs', EventArgs);
ScriptFX.Net.PreHTTPRequestEventArgs.createClass('ScriptFX.Net.PreHTTPRequestEventArgs', EventArgs);
ScriptFX.Net._xmlhttpResponse.createClass('ScriptFX.Net._xmlhttpResponse', null, ScriptFX.Net.IHTTPResponse);
ScriptFX.Net._xmlhttpTransport.createClass('ScriptFX.Net._xmlhttpTransport', ScriptFX.Net.HTTPTransport);
ScriptFX.UI.Animation.createClass('ScriptFX.UI.Animation', null, IDisposable);
ScriptFX.UI.AnimationManager.createClass('ScriptFX.UI.AnimationManager');
ScriptFX.UI.AnimationSequence.createClass('ScriptFX.UI.AnimationSequence', ScriptFX.UI.Animation);
ScriptFX.UI.Behavior.createClass('ScriptFX.UI.Behavior', null, IDisposable, ScriptFX.ISupportInitialize, ScriptFX.INotifyPropertyChanged);
ScriptFX.UI.Color.createClass('ScriptFX.UI.Color');
ScriptFX.UI.Control.createClass('ScriptFX.UI.Control', ScriptFX.UI.Behavior, ScriptFX.INotifyDisposing);
ScriptFX.UI.DOMEventList.createClass('ScriptFX.UI.DOMEventList', null, IDisposable);
ScriptFX.UI.DragDropEventArgs.createClass('ScriptFX.UI.DragDropEventArgs', EventArgs);
ScriptFX.UI.DragDropManager.createClass('ScriptFX.UI.DragDropManager');
ScriptFX.UI._dragDropTracker.createClass('ScriptFX.UI._dragDropTracker', null, ScriptFX.UI.IDragSource);
ScriptFX.UI.Element.createClass('ScriptFX.UI.Element');
ScriptFX.UI.TimedAnimation.createClass('ScriptFX.UI.TimedAnimation', ScriptFX.UI.Animation);
ScriptFX.UI.FadeEffect.createClass('ScriptFX.UI.FadeEffect', ScriptFX.UI.TimedAnimation);
ScriptFX.UI._genericControl.createClass('ScriptFX.UI._genericControl', ScriptFX.UI.Control);
ScriptFX.UI.OverlayBehavior.createClass('ScriptFX.UI.OverlayBehavior', ScriptFX.UI.Behavior);
ScriptFX.UI.PopupBehavior.createClass('ScriptFX.UI.PopupBehavior', ScriptFX.UI.Behavior);
ScriptFX.Application.current = new ScriptFX.Application();
ScriptFX.JSON._dateRegex = null;
ScriptFX.Net.HTTPRequestManager.__requestInvoking = null;
ScriptFX.Net.HTTPRequestManager.__requestInvoked = null;
ScriptFX.Net.HTTPRequestManager._timeoutInterval = 0;
ScriptFX.Net.HTTPRequestManager._activeRequests = [];
ScriptFX.Net.HTTPRequestManager._appIdleHandler = null;
ScriptFX.UI.AnimationManager._fps = 100;
ScriptFX.UI.AnimationManager._activeAnimations = null;
ScriptFX.UI.AnimationManager._timerCookie = 0;
ScriptFX.UI.DragDropManager._dragDropImplementation = null;
ScriptFX.UI.DragDropManager._dropTargets = [];
ScriptFX.UI.DragDropManager._dragStartingHandler = null;
ScriptFX.UI.DragDropManager._dragEndingHandler = null;
ScriptFX.UI.DragDropManager._currentDataObject = null;
ScriptFX.UI.OverlayBehavior._visibilityChangedEventKey$1 = 'visibilityChanged';

// ---- Do not remove this footer ----
// Generated using Script# v0.5.1.0 (http://projects.nikhilk.net)
// -----------------------------------
