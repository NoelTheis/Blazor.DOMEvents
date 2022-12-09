class EventListenerWrapper {
    _listener;
    get listener() {
        return this._listener;
    }
    set listener(value) {
        this._listener = value;
    }
    constructor(listener) {
        this.listener = listener;
    }
}
export function removeEventListener(target, type, wrapper) {
    target.removeEventListener(type, wrapper.listener);
}
export function addEventListener(type, template, dotNetRef) {
    let listener = function (e) {
        var serialized = serializeEvent(template, e);
        dotNetRef.invokeMethodAsync('Invoke', serialized);
    };
    document.addEventListener(type, listener);
    return new EventListenerWrapper(listener);
}
export function getDocument() {
    return document;
}
export function getWindow() {
    return window;
}
function serializeEvent(template, event) {
    populateObject(template, event);
    return JSON.stringify(template);
}
function populateObject(destination, source) {
    if (destination == null || source == null)
        return;
    for (let key in destination) {
        if (key in source) {
            populateProperty(key, destination, source);
        }
    }
}
function populateProperty(key, destination, source) {
    let destProp = destination[key];
    let sourceProp = source[key];
    if (sourceProp === null) {
        destination[key] = null;
        return;
    }
    if (typeof sourceProp === "undefined" || typeof sourceProp === "symbol" || typeof sourceProp === "function")
        return;
    if (destProp === null && typeof sourceProp !== "object") {
        destination[key] = sourceProp;
        return;
    }
    if (typeof destProp === "object" && typeof sourceProp === "object" && destProp != null) {
        if (Array.isArray(destProp) !== Array.isArray(sourceProp))
            return;
        if (Array.isArray(destProp) && Array.isArray(sourceProp)) {
            destination[key] = sourceProp;
            return;
        }
        populateObject(destProp, sourceProp);
        return;
    }
    if (typeof destProp === typeof sourceProp) {
        destination[key] = sourceProp;
        return;
    }
}
//# sourceMappingURL=BlazorDomEvents.js.map