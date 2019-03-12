import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SupplierRoutingModule } from './supplier-routing.module';
import { SupplierListComponent } from './supplier-list/supplier-list.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/compiler/src/core';
import { SearchPipe } from '../../common/pipes/search.pipe';
import { PaginatePipe } from '../../common/pipes/paginate.pipe';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ToastrModule } from 'ng6-toastr-notifications';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../../common/services/api.service';
import { SimpleGridComponent } from '../../common/components/simple-grid/simple-grid.component';
import { LoaderComponent } from '../../common/components/loader/loader.component';
import { httpLoaderService } from '../../common/services/httpLoader.service';

@NgModule({
  declarations: [
    SupplierListComponent
  ],
  imports: [
    CommonModule,
    SupplierRoutingModule,
    FormsModule,
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
export class SupplierModule { }
