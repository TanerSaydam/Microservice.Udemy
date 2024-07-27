import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { ResultModel } from '../models/result.model';
import { ShoppingCartModel } from '../models/shopping-cart.model';
import { api } from '../constants';

@Injectable({
  providedIn: 'root'
})
export class CartsService {
  token = signal<string>("");
  carts = signal<ShoppingCartModel[]>([]);

  constructor(
    private http: HttpClient
  ) {
    if(localStorage.getItem("my-token")){
      this.token.set(localStorage.getItem("my-token")!);

      this.getAll();
    }
  }

  getAll(){
    this.http.get<ResultModel<ShoppingCartModel[]>>(`${api}/shoppingCarts/getall`,{
      headers: {
        "Authorization": "Bearer " + this.token()
      }
    }).subscribe(res=> {
      this.carts.set(res.data!);
    });
  }
}
