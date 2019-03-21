import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomerDetailComponent } from './customer-detail/customer-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../common/shared/shared.module';
import { CustomerRoutingModule } from './customer-routing.module';
import { ToastrModule } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../../common/services/api.service';

@NgModule({
  declarations: [
    CustomerListComponent, 
    CustomerDetailComponent],
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    CustomerRoutingModule,
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
