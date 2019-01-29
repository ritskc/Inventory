import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product.model';
import { ProductService } from './product.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent implements OnInit {

  constructor(private productService: ProductService, private route: ActivatedRoute) { }

  product: Product = new Product();

  ngOnInit() {
    this.productService.getProduct(this.route.snapshot.params.id)
      .subscribe(
        (product) => this.product = product,
        (error) => { console.log(error); }
      );
  }
}
