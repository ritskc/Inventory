import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'paginate'
})
export class PaginatePipe implements PipeTransform {

  transform(value: any, args?: any, args1?: any): any {
    console.log(value);
    console.log(args);
    return value;
  }

}
