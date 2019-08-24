import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { OrderRoutingModule } from './order-routing.module';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../common/shared/shared.module';
import { ToastrModule } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';

@NgModule({
  declarations: [OrderDetailComponent],
  imports: [
    CommonModule,
    FormsModule,
    OrderRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    ToastrModule.forRoot()
  ], 
  providers: [
    httpLoaderService
  ]
})
export class OrderModule { }
