import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  // socket!: WebSocket | null;
  // currentUser = 'User';
  // newMessage = '';
  // messages: string[] = [];

  // ngOnInit(): void {
  //   this.openConnectionToWebSocket();
  // }
  // ngOnDestroy(): void {
  //   this.closeConnectionWebSocket();
  // }

  // openConnectionToWebSocket() {
  //   this.socket = new WebSocket('ws://localhost:5210/ws');
  //   this.socket.onopen = (event) => {
  //     console.log('WebSocket connection opened:', event);
  //   };

  //   this.socket.onmessage = (event) => {
  //     this.messages.push(event.data);
  //   };

  //   this.socket.onclose = (event) => {
  //     console.log('WebSocket connection closed:', event);
  //   };
  // }

  // closeConnectionWebSocket() {
  //   this.socket?.close();
  // }

  // sendMessage() {
  //   if (this.currentUser && this.newMessage) {
  //     const fullMessage = `${this.currentUser}: ${this.newMessage}`;
  //     this.socket?.send(fullMessage);
  //     console.log("send message: " + this.currentUser + " " + this.newMessage );
  //     this.newMessage = '';
  //   }
  // }
  hubConnection?: signalR.HubConnection | null;
  currentUser = '';
  newMessage = '';
  messages: string[] = [];
  ngOnInit(): void {
    this.openConnectionToSignalR();
  }
  ngOnDestroy(): void {
    this.closeConnectionToSignalR();
  }
  openConnectionToSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5210/chatHub')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch((err) => console.log('Error while starting connection: ' + err));

    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      this.messages.push(user + ': ' + message);
    });
  }

  sendMessage() {
    this.hubConnection?.invoke('SendMessage', this.currentUser, this.newMessage)
      .catch(err => console.error(err));
    this.newMessage = '';
  }

    closeConnectionToSignalR() {
    this.hubConnection?.stop();
  }
}
