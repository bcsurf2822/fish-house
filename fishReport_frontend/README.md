# Fish House

A modern web application for managing fish market inventory and pricing. Built with React, this platform enables market administrators to track fish species across multiple locations, update pricing in real-time, and manage market inventories efficiently.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [How to Use](#how-to-use)
  - [Authentication](#authentication)
  - [Managing Markets](#managing-markets)
  - [Managing Fish Inventory](#managing-fish-inventory)
  - [Updating Fish Prices](#updating-fish-prices)
- [Project Structure](#project-structure)
- [API Integration](#api-integration)
- [Screenshots](#screenshots)
- [Development](#development)
- [License](#license)

---

## Overview

Fish House is a comprehensive fish market management system designed for tracking inventory and pricing across multiple market locations. The application provides an intuitive interface for viewing fish species, managing market locations, and maintaining real-time inventory data.

**Live Backend API**: `https://fishwebapp001-c6bvbebffqetg7gq.canadacentral-01.azurewebsites.net`

---

## Features

### Market Management
- View all fish markets in a centralized dashboard
- Create new market locations (authenticated users only)
- Delete existing markets (authenticated users only)
- Track fish inventory per market
- Add and remove fish species from market inventories

### Fish Species Management
- Browse comprehensive fish species catalog
- View detailed fish information (habitat, length, population, lifespan)
- Update fish prices in real-time (authenticated users only)
- Visual fish identification with images from Azure Blob Storage

### Authentication & Security
- JWT-based authentication system
- Role-based access control for administrative operations
- Secure token storage with automatic session management
- Cross-tab authentication synchronization

### User Interface
- Responsive design with Tailwind CSS
- Real-time toast notifications for user feedback
- Modal-based forms for streamlined workflows
- Interactive grid layouts for easy browsing
- Inline editing capabilities

---

## Technology Stack

### Core Technologies
- **React 19.0.0** - Modern UI framework with latest features
- **React Router 7.4.0** - Client-side routing
- **Vite 6.2.0** - Fast build tool and development server
- **Tailwind CSS 4.0.17** - Utility-first CSS framework

### Key Libraries
- **Axios 1.8.4** - Promise-based HTTP client
- **React Hot Toast 2.5.2** - Toast notification system

### Development Tools
- **ESLint 9.21.0** - Code quality and consistency
- **@vitejs/plugin-react-swc 3.8.0** - Fast Refresh with SWC compiler

---

## Getting Started

### Prerequisites

Before running this application, ensure you have the following installed:

- **Node.js** (v18 or higher recommended)
- **npm** (comes with Node.js)
- A modern web browser (Chrome, Firefox, Safari, or Edge)

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd fishReport_frontend
```

2. Install dependencies:
```bash
npm install
```

### Running the Application

#### Development Mode

Start the development server with hot module replacement:

```bash
npm run dev
```

The application will be available at `http://localhost:5173` (or another port if 5173 is occupied).

#### Production Build

Create an optimized production build:

```bash
npm run build
```

Preview the production build locally:

```bash
npm run preview
```

#### Linting

Run ESLint to check code quality:

```bash
npm run lint
```

---

## How to Use

### Authentication

#### Logging In

1. Navigate to the application in your web browser
2. Locate the login form in the top-right corner of the navigation bar
3. Enter your username and password
4. Click the "Login" button
5. Upon successful authentication, you'll see a welcome message with your username

**Note**: Authentication is required for administrative operations like creating markets, deleting markets, updating prices, and modifying inventories.

#### Logging Out

1. Click the "Logout" button in the top-right corner
2. Your session will be cleared, and you'll need to log in again for protected operations

---

### Managing Markets

#### Viewing Markets

1. Navigate to the **Markets** page (home page at `/`)
2. All available markets are displayed in the left sidebar
3. Click on any market to view its fish inventory

#### Creating a New Market

1. Ensure you are logged in
2. On the Markets page, click the "Add Market" button
3. In the modal form, enter:
   - **Market Name**: The name of the new market
   - **Location**: The geographic location of the market
4. Click "Add Market" to create
5. The new market will appear in the market list

#### Deleting a Market

1. Ensure you are logged in
2. Select the market you want to delete from the list
3. Click the "Delete Market" button
4. Confirm the deletion in the confirmation modal
5. The market will be removed from the system

---

### Managing Fish Inventory

#### Adding Fish to a Market

1. Ensure you are logged in
2. Select a market from the list
3. Click the "Add Fish" button
4. In the dropdown, browse available fish species with images
5. Select the fish you want to add
6. Click "Add Fish to Market"
7. The fish will immediately appear in the market's inventory

#### Removing Fish from a Market

1. Ensure you are logged in
2. Select a market and view its current inventory
3. Locate the fish you want to remove
4. Click the "Delete" button next to that fish
5. The fish will be removed from the market's inventory

**Note**: Removing a fish from a market does not delete the fish species from the system, only from that specific market's inventory.

---

### Updating Fish Prices

#### Editing Fish Prices

1. Navigate to the **Fish** page from the navigation menu
2. Ensure you are logged in
3. Browse the fish species grid
4. Find the fish whose price you want to update
5. Click the "Edit Price" button next to the current price
6. Enter the new price in the input field
7. Click "Save" to update the price
8. The new price will be saved and displayed immediately

**Note**: Price changes affect the fish globally across all markets.

---

## Project Structure

```
fishReport_frontend/
├── api/                          # API service layer
│   ├── auth.js                   # Authentication endpoints
│   ├── fish.js                   # Fish species API calls
│   └── markets.js                # Market management API calls
├── components/                   # React components
│   ├── Nav.jsx                   # Navigation bar with authentication
│   ├── fish/
│   │   └── FishCollection.jsx    # Fish species listing and management
│   ├── home/
│   │   ├── Home.jsx              # Home page wrapper
│   │   └── board/
│   │       └── FishBoard.jsx     # Market inventory dashboard
│   └── market/
│       └── AddMarketForm.jsx     # Market creation modal form
├── contexts/                     # React Context providers
│   └── authContext.jsx           # Global authentication state
├── src/
│   ├── main.jsx                  # Application entry point
│   ├── App.jsx                   # Root component with routing
│   ├── index.css                 # Global Tailwind styles
│   └── assets/
│       └── fishImageMap.js       # Fish image URL mappings
├── public/                       # Static assets
├── index.html                    # HTML entry point
├── vite.config.js                # Vite configuration
├── eslint.config.js              # ESLint rules
├── tailwind.config.js            # Tailwind CSS configuration
└── package.json                  # Dependencies and scripts
```

---

## API Integration

The application integrates with a RESTful backend API hosted on Azure:

**Base URL**: `https://fishwebapp001-c6bvbebffqetg7gq.canadacentral-01.azurewebsites.net`

### Key Endpoints

#### Authentication
- `POST /api/authentication/login` - User login with JWT token response

#### Fish Management
- `GET /api/fish/getall` - Retrieve all fish species with full details
- `GET /api/fish/fishinventory` - Get simplified fish list for inventory operations
- `PATCH /api/fish/updatepartial/{id}` - Update fish price (requires authentication)

#### Market Management
- `GET /api/fishmarket/inventory` - Get all markets with their inventories
- `POST /api/fishmarket/createnew` - Create a new market (requires authentication)
- `DELETE /api/fishmarket/delete/{marketId}` - Delete a market (requires authentication)
- `POST /api/fishmarket/addtoinventory/{marketId}/{speciesId}` - Add fish to market
- `DELETE /api/fishmarket/deletefrominventory/{marketId}/{speciesId}` - Remove fish from market

All authenticated endpoints require a Bearer token in the Authorization header.

---

## Screenshots

> Add screenshots here showing:
> - Market dashboard with inventory list
> - Fish collection grid view
> - Add market modal form
> - Login interface
> - Fish price editing interface
> - Add fish to market dropdown

---

## Development

### Code Style

This project follows consistent logging conventions:

```javascript
console.log('[FILENAME-FUNCTION] description and log of what is happening');
```

Example:
```javascript
console.log('[FishBoard.jsx-handleDeleteFish] Deleting fish with id: 123');
```

### State Management

The application uses React Context API for global authentication state and component-level state for UI and data management.

### Error Handling

All API calls include error handling with toast notifications to provide immediate user feedback.

### Image Assets

Fish images are hosted on Azure Blob Storage and mapped using the `getFishImage` utility function. Images automatically fall back to a default tuna image if a specific fish image is not found.

---

## License

This project is private and proprietary.

---

**Project Version**: 0.0.0

**Last Updated**: 2025-10-29
