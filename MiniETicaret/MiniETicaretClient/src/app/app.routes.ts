import { Routes } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { LayoutsComponent } from './components/layouts/layouts.component';
import { HomeComponent } from './components/home/home.component';
import { ShoppingCartsComponent } from './components/shopping-carts/shopping-carts.component';
import { OrdersComponent } from './components/orders/orders.component';

export const routes: Routes = [
    {
        path:"register",
        component: RegisterComponent
    },
    {
        path:"login",
        component: LoginComponent
    },
    {
        path:"",
        component: LayoutsComponent,
        children: [
            {
                path: "",
                component: HomeComponent
            },
            {
                path: "shopping-carts",
                component: ShoppingCartsComponent
            },
            {
                path: "orders",
                component: OrdersComponent
            }
        ]
    },
];
