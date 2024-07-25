import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  todos: TodoModel[] = [];
  work: string = "";
  name: string = "";
  categories: CategoryModel[] = [];

  constructor(
    private http: HttpClient
  ){
    this.getAllTodos();
    this.getAllCategories();
  }

  getAllTodos(){
    this.http.get<TodoModel[]>("http://localhost:5000/api/todos/getall").subscribe(res=> {
      this.todos = res;
    })
  }

  saveTodo(){
    this.http.get("http://localhost:5000/api/todos/create?work=" + this.work).subscribe(()=> {
      this.getAllTodos();
    })
  }

  getAllCategories(){
    this.http.get<CategoryModel[]>("http://localhost:5000/api/categories/getall").subscribe(res=> {
      this.categories = res;
    })
  }

  saveCategory(){
    const data = {
      name: this.name
    }
    this.http.post("http://localhost:5000/api/categories/create", data).subscribe(()=> {
      this.getAllCategories();
    })
  }
}
export class TodoModel{
  id: number = 0;
  work: string = "";
}


export class CategoryModel{
  id: number = 0;
  name: string = "";
}