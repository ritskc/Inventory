import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CompanyRoutingModule } from './company-routing.module';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyDetailComponent } from './company-detail/company-detail.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SimpleGridComponent } from '../common/components/simple-grid/simple-grid.component';
import { SearchPipe } from '../common/pipes/search.pipe';

@NgModule({
  declarations: [
    CompanyListComponent, 
    CompanyDetailComponent, 
    SimpleGridComponent,
    SearchPipe
  ],
  imports: [
    CommonModule,
    CompanyRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class CompanyModule { }
