interface IDotNetObjectReference {
    invokeMethodAsync: (methodName: string, param: string) => void
}

class EventListenerWrapper {
    private _listener: EventListenerOrEventListenerObject;

    public get listener(): EventListenerOrEventListenerObject {
        return this._listener;
    }
    private set listener(value: EventListenerOrEventListenerObject) {
        this._listener = value;
    }

    constructor(listener: EventListenerOrEventListenerObject) {
        this.listener = listener;
    }    
}

export function removeEventListener(
    target: EventTarget,
    type: string,
    wrapper: EventListenerWrapper)
{
    target.removeEventListener(
        type,
        wrapper.listener);
}

export function addEventListener(
    type: string,
    template: object,
    dotNetRef: IDotNetObjectReference)
    : EventListenerWrapper
{
    let listener = function (e) {
        var serialized = serializeEvent(template, e);
        dotNetRef.invokeMethodAsync('Invoke', serialized);
    }
    document.addEventListener(
        type,
        listener);

    return new EventListenerWrapper(listener);
}

export function getDocument(): Document
{
    return document;
}  

export function getWindow(): Window
{
    return window;
}  

function serializeEvent(template: object, event: object): string
{
    populateObject(template, event);
    return JSON.stringify(template);
}

function populateObject(destination: object, source: object) : void
{
    if (destination == null || source == null)
        return;

    for (let key in destination) {
        if (key in source) {
            populateProperty(key, destination, source);
        }
    }
}

function populateProperty(key: string, destination: object, source: object): void
{
    let destProp = destination[key];
    let sourceProp = source[key];

    //if sourceProp is null, just set destProp to null and return
    if (sourceProp === null) {
        destination[key] = null;
        return;
    }  
        
    //return if sourceProp type is no use to us
    if (typeof sourceProp === "undefined" || typeof sourceProp === "symbol" || typeof sourceProp === "function")
        return;

    //if destProp is null and sourceProp is one of the primitive types left
    //(bool, number, bigint, string) set destProp to sourceProp and return
    if (destProp === null && typeof sourceProp !== "object")
    {
        destination[key] = sourceProp;
        return;
    }

    //both destProp and sourceProp are objects
    if (typeof destProp === "object" && typeof sourceProp === "object" && destProp != null)
    {

        //one of the property is an array and the other is not, return
        if (Array.isArray(destProp) !== Array.isArray(sourceProp))
            return;

        //if array just copy reference
        if (Array.isArray(destProp) && Array.isArray(sourceProp))
        {
            destination[key] = sourceProp;
            return;
        }

        populateObject(destProp, sourceProp);
        return;
    }

    //destProp and sourceProp are of the same primitive type (bool, number, bigint, string)
    if (typeof destProp === typeof sourceProp) {
        destination[key] = sourceProp;
        return;
    }
}