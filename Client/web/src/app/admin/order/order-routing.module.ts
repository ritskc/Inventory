import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { OrderDetailComponent } from './order-detail/order-detail.component';

const routes: Routes = [{
  path: '',
  data: {
    title: 'Orders'
  },
  children: [{
    path: 'detail/:from/:id/:action/:orderId',
    component: OrderDetailComponent
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrderRoutingModule { }
