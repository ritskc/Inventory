import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InvoiceRoutingModule } from './invoice-routing.module';
import { InvoiceListComponent } from './invoice-list/invoice-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../common/shared/shared.module';
import { ToastrModule } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../../common/services/api.service';
import { InvoiceService } from './invoice.service';
import { UploadInvoiceComponent } from './upload-invoice/upload-invoice.component';
import { ReceiveComponent } from './receive/receive.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ModalsComponent } from '../../views/notifications/modals.component';
import { ContainerListComponent } from '../container/list/list.component';

@NgModule({
  declarations: [
    InvoiceListComponent,
    UploadInvoiceComponent,
    ReceiveComponent,
    ModalsComponent,
    ContainerListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    InvoiceRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    ToastrModule.forRoot(),
    ModalModule.forRoot()
  ], 
  providers: [
    httpLoaderService,
    InvoiceService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
  ]
})
export class InvoiceModule { }
