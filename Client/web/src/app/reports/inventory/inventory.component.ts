import { Component, OnInit } from '@angular/core';
import { InventoryService } from './inventory.service';
import { Part } from '../../models/part.model';

@Component({
  selector: 'app-inventory',
  templateUrl: './inventory.component.html',
  styleUrls: ['./inventory.component.scss']
})
export class InventoryComponent implements OnInit {

  parts: Part[];

  constructor(private inventoryService: InventoryService) { 
    this.parts = [];
  }

  ngOnInit() { 
    var result = this.inventoryService.getAllParts()
      .subscribe((response) => {
        this.parts = response;
      });
  }

}
