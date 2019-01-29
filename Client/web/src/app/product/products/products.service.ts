import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Product } from '../../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {

  constructor(private apiService: ApiService, private config: ConfigService) { }

  getAllProducts() {
    return this.apiService.get<Product[]>(this.config.Settings.apiServerHost + this.config.Settings.partsUri);
  }
}
