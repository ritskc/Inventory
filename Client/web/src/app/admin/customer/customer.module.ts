import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CustomerRoutingModule } from './customer-routing.module';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../../common/services/api.service';
import { SharedModule } from '../../common/shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrModule } from 'ng6-toastr-notifications';
import { CustomerDetailComponent } from './customer-detail/customer-detail.component';
import { PurchaseOrdersComponent } from './purchase-orders/purchase-orders.component';

@NgModule({
  declarations: [
    CustomerListComponent,
    CustomerDetailComponent,
    PurchaseOrdersComponent
  ],
  imports: [
    CommonModule,
    CustomerRoutingModule,
    FormsModule,
    SharedModule,
    ReactiveFormsModule,
    ToastrModule.forRoot()
  ],
  providers: [
    httpLoaderService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
  ]
})
export class CustomerModule { }
