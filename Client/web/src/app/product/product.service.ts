import { Injectable } from '@angular/core';
import { ApiService } from '../common/services/api.service';
import { Product } from '../models/product.model';
import { ConfigService } from '../config/config.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private apiService: ApiService, private config: ConfigService) { }

  getAllProducts() {
    return this.apiService.get<Product[]>(this.config.Settings.apiServerHost + this.config.Settings.partsUri);
  }

  getProduct(id: number): Observable<Product> {
    return this.apiService.get<Product>(this.config.Settings.apiServerHost + this.config.Settings.partsUri + '/' + id);
  }

  addProduct(product: Product) {
    return this.apiService.post(product, this.config.Settings.apiServerHost + this.config.Settings.partsUri);
  }

  editProduct(product: Product) {
    return this.apiService.put(product, this.config.Settings.apiServerHost + this.config.Settings.partsUri);
  }
}