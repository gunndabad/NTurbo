import { EventTarget } from "event-target-shim/es5";
import signalR from "@microsoft/signalr";
import { connectStreamSource, disconnectStreamSource } from "@hotwired/turbo";
import { StreamSource } from "@hotwired/turbo/dist/types/core/types";

const callbackMethodName = "ReceiveStreamElement";

type EventMap = {
  message: MessageEvent
};

export class SignalRStreamObserver extends EventTarget<EventMap> implements StreamSource {
  _connection: signalR.HubConnection;
  _attached: boolean = false;

  constructor(connection: signalR.HubConnection) {
    super();
    this._connection = connection;
    this._receiveStreamElement = this._receiveStreamElement.bind(this);
  }

  get connection() {
    return this._connection;
  }

  attach() {
    if (this._attached === true) {
      return;
    }

    const connection = this._connection;
    connection.on(callbackMethodName, this._receiveStreamElement);

    connectStreamSource(this);

    this._attached = true;
  }

  detach() {
    if (this._attached !== true) {
      return;
    }

    disconnectStreamSource(this);

    const connection = this._connection;
    connection.off(callbackMethodName, this._receiveStreamElement);

    this._attached = false;
  }

  _receiveStreamElement(element: string) {
    const event = new MessageEvent("message", { data: element });
    this.dispatchEvent(event);
  }
}
