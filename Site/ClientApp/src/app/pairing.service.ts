import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";
import { Subject, Observable, from } from 'rxjs';
import { tap, switchMap, map } from "rxjs/operators";
import { HttpClient } from '@angular/common/http';
import { TagPlaceholder } from '@angular/compiler/src/i18n/i18n_ast';

const apiRoute = "pairing";

@Injectable({
  providedIn: 'root'
})
export class PairingService {

  private hub: HubConnection;
  private subject = new Subject<string>();

  public get HubObservable(): Observable<string> {
    return this.subject.asObservable();
  }

  constructor(private http: HttpClient) { }

  public startConnection(): void {

    this.hub = new HubConnectionBuilder().withUrl("hub").build();
    this.hub.start()
      .then(() => console.log('Started SignalR hub successfully'))
      .catch(err => console.log('Error starting SignalR hub', err));

    this.hub.on('transferUrl', (url: string) => {
      console.log('transferUrl message received', url);
      this.subject.next(url);
    });
  }

}
