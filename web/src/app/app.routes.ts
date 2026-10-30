import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { LoginPage } from './features/auth/login.page';
import { ProductLogsPage } from './features/logs/product-logs.page';
import { ProductDetailsPage } from './features/products/product-details.page';
import { ProductFormPage } from './features/products/product-form.page';
import { ProductsPage } from './features/products/products.page';
import { AppLayout } from './layout/app-layout';

export const routes: Routes = [
  { path: 'login', component: LoginPage },
  {
    path: '',
    component: AppLayout,
    canActivateChild: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'products' },
      { path: 'products', component: ProductsPage },
      { path: 'products/new', component: ProductFormPage },
      { path: 'products/:id/edit', component: ProductFormPage },
      { path: 'products/:id/logs', component: ProductLogsPage },
      { path: 'products/:id', component: ProductDetailsPage },
      { path: 'logs', component: ProductLogsPage },
    ],
  },
  { path: '**', redirectTo: '' },
];
