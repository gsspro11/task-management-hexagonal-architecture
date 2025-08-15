import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./board.component').then(m => m.BoardComponent),
    data: {
      title: 'Board'
    }
  }
];


