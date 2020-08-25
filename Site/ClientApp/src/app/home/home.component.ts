import { Component, OnInit } from '@angular/core';
import { PairingService } from '../pairing.service';
import { tap, switchMap } from 'rxjs/operators';
import { APP_BASE_HREF } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  public id: string;
  public loading: boolean;

  constructor(private pairingService: PairingService) {

  }

  ngOnInit(): void {
    this.id = "ABCD1234";
    this.pairingService.startConnection();
    this.pairingService.HubObservable.subscribe(url => {
      alert(`Navigating to URL: ${url}`);
      location.href = url;
    });
  }
}