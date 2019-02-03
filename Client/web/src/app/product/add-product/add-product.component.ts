import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.scss']
})
export class AddProductComponent implements OnInit {

  product: Product;
  addProductForm: FormGroup;
  submitted: boolean = false;

  constructor(private addProductBuilder: FormBuilder, private productService: ProductService) { 
    
    this.product = new Product();

    this.addProductForm = this.addProductBuilder.group({
      productName: ['', Validators.required],
      productDescription: ['', Validators.required]
    });
  }

  ngOnInit() {
  }

  get f() {
    return this.addProductForm.controls;
  }

  toggleSampleSelction() {
    this.product.isSample = !this.product.isSample;
  }

  save() {
    this.submitted = true;
    if (this.addProductForm.invalid) return;

    this.product.name = this.addProductForm.value.productName;
    this.product.description = this.addProductForm.value.productDescription;
    
    this.productService.addProduct(this.product)
      .subscribe(
        (response) => { 
          console.log(response);
          
        },
        (error) => { console.log(error) }
      );
  }

}
