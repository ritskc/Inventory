import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../user/auth.guard';
import { ProductsComponent } from './products/products.component';
import { ProductComponent } from './product/product.component';
import { AddProductComponent } from './add-product/add-product.component';

const routes: Routes = [{
  path: '',
  //canActivate: [AuthGuard],
  data: {
    title: 'Products'
  },
  children: [{
    path: '',
    component: ProductsComponent
  }, {
    path: 'add',
    component: AddProductComponent,
    data: {
      title: 'Add Product'
    }
  }, {
    path: 'product/:id',
    component: ProductComponent,
    data: {
      title: 'Product'
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductRoutingModule { }
