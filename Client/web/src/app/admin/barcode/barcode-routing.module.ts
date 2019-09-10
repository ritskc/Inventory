import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BarcodeComponent } from './barcode/barcode.component';
import { QrCodeComponent } from './qr-code/qr-code.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Barcode'
  }, 
  children: [{
    path: '',
    component: BarcodeComponent
  }, {
    path: 'qrcode',
    component: QrCodeComponent, 
    data: {
      title: 'QR Code'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BarcodeRoutingModule { }
