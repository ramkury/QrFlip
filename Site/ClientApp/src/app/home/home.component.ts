import { Component, OnDestroy, OnInit } from '@angular/core';
import { PairingService } from '../pairing.service';
import { tap, switchMap } from 'rxjs/operators';
import { APP_BASE_HREF } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RedirectRequest } from '../api/pairing';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit, OnDestroy {
  private $toUnsubscribe: Subscription[] = [];
  private targetId = '';

  public id: string;
  public form: FormGroup;
  public get qrCodeData(): string {
    return `https://localhost:5001?targetId=${this.id}`;
  }

  constructor(
    private pairingService: PairingService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
  ) {

  }

  ngOnInit(): void {
    this.setupPairing();
    this.buildForm();
  }

  public submitForm(): void {
    let model = this.form.value;
    let request: RedirectRequest = {
      ClientId: model['targetId'],
      DestinationUrl: model['url']
    };
    this.pairingService.sendRedirect(request).subscribe();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      'targetId': [this.targetId, [Validators.required, Validators.maxLength(8)]],
      'url': ['', [
        Validators.required,
      ]],
    })
  }

  private setupPairing(): void {
    this.pairingService.startConnection();
    this.pairingService.UrlChanged.subscribe(url => {
      console.log(url);
      location.href = url;
    });
    this.pairingService.AliasChanged.subscribe(alias => this.id = alias);
    this.$toUnsubscribe.push(this.route.queryParamMap.subscribe(
      p => this.updateTargetId(p.get('targetId'))
    ));
  }

  private updateTargetId(newTargetId: string): void {
    this.targetId = newTargetId;
    if (this.form) {
      this.form.patchValue({targetId: newTargetId});
    }
  }

  ngOnDestroy(): void {
    for (let s of this.$toUnsubscribe) {
      s.unsubscribe();
    }
  }
}
