import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product.model';
import { ProductsService } from './products.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  products: Product[];

  constructor(private productsService: ProductsService, private router: Router) { 
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
}
