import { Injectable } from '@angular/core';
import { ApiService } from '../common/services/api.service';
import { Product } from '../models/product.model';
import { ConfigService } from '../config/config.service';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private apiService: ApiService, private config: ConfigService) { }

  addProduct(product: Product) {
    return this.apiService.post(product, this.config.Settings.apiServerHost + this.config.Settings.partsUri);
  }
}
