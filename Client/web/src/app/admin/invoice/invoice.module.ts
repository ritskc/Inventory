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

@NgModule({
  declarations: [
    InvoiceListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    InvoiceRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    ToastrModule.forRoot()
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
