import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InvoiceListComponent } from './invoice-list/invoice-list.component';
import { UploadInvoiceComponent } from './upload-invoice/upload-invoice.component';
import { ReceiveComponent } from './receive/receive.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Invoice'
  }, children: [
    {
      path: '',
      component: InvoiceListComponent,
      data: {
        title: 'List'
      }, 
    },
    {
      path: 'receive',
      component: ReceiveComponent,
      data: {
        title: 'Receive'
      }
    },
    {
      path: 'receive/:id',
      component: ReceiveComponent,
      data: {
        title: 'Receive'
      }
    },
    {
      path: 'upload/:id/:mode',
      component: UploadInvoiceComponent,
      data: {
        title: 'Create Invoice'
      }
    },
    {
      path: 'upload/:id/:mode/:invoiceId',
      component: UploadInvoiceComponent,
      data: {
        title: 'Edit Invoice'
      }
    }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InvoiceRoutingModule { }
