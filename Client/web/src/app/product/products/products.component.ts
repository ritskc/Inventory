import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product.model';
import { Router } from '@angular/router';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  products: Product[];

  constructor(private productsService: ProductService, private router: Router) { 
    this.products = [];
  }

  ngOnInit() {
    this.productsService.getAllProducts()
      .subscribe((response) => {
        this.products = response
      },
    () => {})
  }

  productSelected(id: number) {
    this.router.navigateByUrl(`/products/product/${id}`);
  }

  addProduct() {
    this.router.navigateByUrl(`/products/add`);
  }
}