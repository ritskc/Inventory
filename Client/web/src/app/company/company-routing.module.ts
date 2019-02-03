import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyDetailComponent } from './company-detail/company-detail.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Companies'
  },
  children: [{
    path: '',
    component: CompanyListComponent
  }, {
    path: 'detail/:action/:id',
    component: CompanyDetailComponent,
    data: {
      title: 'Company Detail'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompanyRoutingModule { }
