# Aggregation Backend

A simple backend project structured with Domain-Driven Design (DDD) architecture that demonstrates combining data from 3 external apis.

---

## Table of Contents

- [Motivation](#motivation)  
- [Architecture](#architecture)  
- [Features](#features)  
- [Getting Started](#getting-started)  
- [Configuration](#configuration)  
- [Usage](#usage)  
- [Testing](#testing)  
- [Folder Structure](#folder-structure)  
- [Contributing](#contributing)  
- [License](#license)  


## Motivation

In many real-world scenarios, backend systems must **aggregate** and **normalize** data from multiple external APIs or services.  
This project aims to demonstrate a clean, maintainable approach using DDD principles to coordinate and unify disparate sources into a coherent domain model.

## Architecture

- Based on DDD (Domain-Driven Design) to separate concerns (domain, application, infrastructure)  
- Each external data source has its own adapter/integration layer  
- A coordination or “aggregator” component merges or reconciles data  
- Emphasis on clean code, testability, and clear boundaries  

## Features

- Integration with three external data sources  
- Data transformation, normalization, and merging logic  
- Clear separation of domain logic and infrastructure concerns  
- Easily extensible to add more sources or change existing ones  
- Basics of error handling and fallback strategies  

## Getting Started

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)  
- (Optional) Docker, Postman or similar for testing endpoints or integrations  

### Clone & Build

```bash
git clone https://github.com/pauldin91/aggregation_backend.git
cd aggregation_backend
dotnet build
