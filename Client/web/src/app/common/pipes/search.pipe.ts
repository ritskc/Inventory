import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'search'
})
export class SearchPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    if (!args) return value;
    
    return value.filter(item => {
      var jsonData = JSON.stringify(item).toUpperCase();
      return jsonData.indexOf(args.toUpperCase()) > 0 ? value: null;
    });
  }
}