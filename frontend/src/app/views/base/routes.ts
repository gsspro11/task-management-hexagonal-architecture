import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Base'
    },
    children: [
      {
        path: '',
        redirectTo: 'cards',
        pathMatch: 'full'
      },
      {
        path: 'board',
        loadComponent: () => import('./board/board.component').then(m => m.BoardComponent),
        data: {
          title: 'Board'
        }
      }
    ]
  }
];


