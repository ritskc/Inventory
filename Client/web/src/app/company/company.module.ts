import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CompanyRoutingModule } from './company-routing.module';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyDetailComponent } from './company-detail/company-detail.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SimpleGridComponent } from '../common/components/simple-grid/simple-grid.component';
import { SearchPipe } from '../common/pipes/search.pipe';
import { PaginatePipe } from '../common/pipes/paginate.pipe';
import { ToastrModule } from 'ng6-toastr-notifications';
import { ReportComponent } from '../common/components/report/report.component';
import { LoaderComponent } from '../common/components/loader/loader.component';
import { httpLoaderService } from '../common/services/httpLoader.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from '../common/services/api.service';
import { SharedModule } from '../common/shared/shared.module';

@NgModule({
  declarations: [
    CompanyListComponent, 
    CompanyDetailComponent,
    ReportComponent
  ],
  imports: [
    CommonModule,
    CompanyRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ToastrModule.forRoot(),
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
export class CompanyModule { }
