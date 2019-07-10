import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InvoiceListComponent } from './invoice-list/invoice-list.component';
import { UploadInvoiceComponent } from './upload-invoice/upload-invoice.component';

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
      }
    }, {
      path: 'upload/:id',
      component: UploadInvoiceComponent,
      data: {
        title: 'Upload'
      }
    }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InvoiceRoutingModule { }
