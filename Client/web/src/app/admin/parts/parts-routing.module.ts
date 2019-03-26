import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PartListComponent } from './part-list/part-list.component';
import { PartDetailComponent } from './part-detail/part-detail.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Parts'
  },
  children: [
    {
      path: '',
      component: PartListComponent
    }, {
      path: 'detail/:action/:id',
      component: PartDetailComponent,
      data: {
        title: 'Parts Detail'
      }
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PartsRoutingModule { }
