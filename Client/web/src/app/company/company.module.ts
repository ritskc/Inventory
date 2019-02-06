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

@NgModule({
  declarations: [
    CompanyListComponent, 
    CompanyDetailComponent, 
    SimpleGridComponent,
    SearchPipe,
    PaginatePipe
  ],
  imports: [
    CommonModule,
    CompanyRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot(),
  ]
})
export class CompanyModule { }
