import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BarcodeRoutingModule } from './barcode-routing.module';
import { QrCodeComponent } from './qr-code/qr-code.component';
import { BarcodeComponent } from './barcode/barcode.component';
import { NgxBarcodeModule } from 'ngx-barcode';
import { FormsModule } from '@angular/forms';
import { QRCodeModule } from 'angularx-qrcode';
import { BarecodeScannerLivestreamModule } from 'ngx-barcode-scanner';
import { ToastrModule } from 'ng6-toastr-notifications';
import { SharedModule } from '../../common/shared/shared.module';

@NgModule({
  declarations: [
    QrCodeComponent, 
    BarcodeComponent
  ],
  imports: [
    BarcodeRoutingModule,
    BarecodeScannerLivestreamModule,
    CommonModule,
    FormsModule,
    NgxBarcodeModule,
    QRCodeModule,
    SharedModule,
    ToastrModule.forRoot(),
  ]
})
export class BarcodeModule { }
