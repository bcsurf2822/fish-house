# Fish House

A cloud-native fish market application demonstrating enterprise-grade AWS infrastructure deployment with a modern .NET backend and React frontend.

## Overview

Fish House is a full-stack web application built to showcase production-level cloud architecture and modern application development practices. The project implements a fish species and market inventory management system deployed on AWS using infrastructure-as-code principles and industry-standard security patterns.

**Primary Focus:** This project emphasizes AWS cloud infrastructure design, multi-tier security architecture, and cost-optimized deployment strategies while maintaining production-grade standards.

## Architecture

### Cloud Infrastructure (AWS)

The application is deployed on AWS using a custom VPC with defense-in-depth security and high availability across multiple Availability Zones.

**Network Architecture:**

- Custom VPC
- Multi-tier subnet design across 2 Availability Zones
- Public subnets for application tier and load balancing
- Private subnets for database isolation
- Internet Gateway for public internet access
- Cost-optimized routing

**Security Architecture:**

Three-layer security group implementation:

1. **Load Balancer Layer** - Accepts HTTP/HTTPS from internet (0.0.0.0/0)
2. **Application Layer** - Only accepts traffic from load balancer security group
3. **Database Layer** - Only accepts SQL Server connections (port 1433) from application security group

**Compute Resources:**

- AWS Elastic Beanstalk managing EC2 instances
- Platform: Windows Server 2022 with IIS 10.0
- Instance type: t2.micro (free tier eligible)
- Single instance deployment for cost optimization

**Database:**

- Amazon RDS SQL Server Express Edition
- Instance class: db.t3.micro
- Multi-AZ capable (currently single-AZ)
- Deployed in private subnets with zero public accessibility
- Automated backups with 7-day retention

**Load Balancing:**

- Application Load Balancer distributing traffic across availability zones
- Health checks for automatic failover
- Internet-facing endpoint for public access

### Backend API

The Fish House API is a RESTful web service built with .NET 8 and Entity Framework Core, providing comprehensive CRUD operations for fish species and market inventory management.

**Technology Stack:**

- .NET 8 Web API (ASP.NET Core)
- Entity Framework Core 9.0 with SQL Server provider
- JWT Bearer token authentication
- AutoMapper for DTO transformations
- Swagger/OpenAPI documentation
- JSON Patch support for partial updates

**Key Features:**

- RESTful API design with resource-based routing
- Repository pattern implementation for data access abstraction
- Generic repository base class with specialized implementations
- Automatic database migrations on application startup
- Many-to-many relationship management (Species ↔ Markets)
- Database seeding with sample data (18 fish species, 3 markets)
- CORS configuration for cross-origin frontend integration
- Comprehensive input validation and error handling

**API Capabilities:**

Fish Species Management:

- Retrieve all fish species with detailed information
- Get individual species by ID
- Create new species entries
- Partial updates via JSON Patch (protected)
- Delete species (protected)
- Query species with market relationships

Fish Market Management:

- List all markets with inventory
- Get specific market details
- Create new markets (protected)
- Update market information
- Add/remove species from market inventory (protected)
- Delete markets (protected)
- Lightweight inventory queries for performance

Authentication:

- JWT

**Database Schema:**

Three core entities with explicit many-to-many junction table:

- **Species**: Fish species information (name, habitat, length, population, price)
- **FishMarket**: Market details (name, location)
- **FishMarketInventory**: Junction table linking species to markets

### Frontend Application

React-based single-page application providing an intuitive interface for browsing fish species and managing market inventories.

**Features:**

- Browse fish species catalog with detailed information
- View market locations and inventory
- Search and filter capabilities
- Responsive design for desktop and mobile
- Integration with Fish House API via RESTful calls
- CORS-enabled communication with backend

**Deployment:**

- Hosted on Netlify CDN
- Production URL: <https://fish-house-demo.netlify.app>
- Continuous deployment from Git repository

## AWS Services

### Core Infrastructure Services

**Amazon VPC**

- Custom network environment with complete control over IP addressing
- Multi-AZ deployment for high availability
- Subnet segregation for security isolation
- Route table management for traffic control
- Network ACLs for additional security layer

**Amazon EC2**

- Via AWS Elastic Beanstalk for managed infrastructure
- Windows Server 2022 instances running IIS 10.0
- Auto-assigned public IP addresses in public subnets
- T2.micro instance type (free tier eligible)
- Security group protection at instance level

**AWS Elastic Beanstalk**

- Platform-as-a-Service for .NET application deployment
- Automated provisioning and scaling
- Health monitoring and automatic recovery
- Load balancer integration
- Environment configuration management
- Application versioning and rollback capabilities

**Application Load Balancer**

- Layer 7 load balancing across availability zones
- HTTP/HTTPS listener configuration
- Health check integration
- Internet-facing deployment
- Security group protection

**Amazon RDS (Relational Database Service)**

- Managed SQL Server Express Edition
- Automated backup and point-in-time recovery
- Multi-AZ capability for production scenarios
- Patch management and maintenance windows
- Performance insights and monitoring

**Amazon S3 (Simple Storage Service)**

- Elastic Beanstalk deployment package storage
- Application version artifacts
- Automatic replication and durability

### Networking Services

**Internet Gateway**

- Provides internet connectivity to VPC
- Enables public subnet instances to communicate with internet
- Horizontally scaled and highly available

**Route Tables**

- Public route table directing traffic to Internet Gateway
- Private route tables for isolated database subnets
- Custom routing policies for security

**Security Groups**

- Stateful firewall rules at instance level
- Three-tier security group chain implementation

**Subnets**

- 4 subnets across 2 Availability Zones
- 2 public subnets
- 2 private subnets
- Automatic public IP assignment enabled on public subnets

### Management & Monitoring

**AWS CloudWatch** (Via Elastic Beanstalk)

- Application and infrastructure metrics
- Log aggregation and monitoring

**AWS IAM (Identity and Access Management)**

- Service roles for Elastic Beanstalk
- Instance profiles for EC2 instances

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     INTERNET (Users)                            │
└────────────────────────────┬────────────────────────────────────┘
                             │
                    ┌────────▼────────┐
                    │ Internet Gateway│
                    └────────┬────────┘
                             │
        ┌────────────────────┴────────────────────┐
        │          PUBLIC SUBNETS (2 AZs)         │
        │                                         │
        │  ┌──────────────────────────────────┐  │
        │  │  Application Load Balancer       │  │
        │  │  (sg-087b06f8d60622920)         │  │
        │  └───────────────┬──────────────────┘  │
        │                  │                      │
        │  ┌───────────────▼──────────────────┐  │
        │  │  .NET 8 API (Elastic Beanstalk) │  │
        │  │  Windows Server 2022 + IIS      │  │
        │  │  (sg-0ae000b04b5a73bc9)         │  │
        │  └───────────────┬──────────────────┘  │
        └──────────────────┼──────────────────────┘
                           │ Port 1433
        ┌──────────────────▼──────────────────────┐
        │        PRIVATE SUBNETS (2 AZs)          │
        │                                         │
        │  ┌──────────────────────────────────┐  │
        │  │  RDS SQL Server Express          │  │
        │  │  (sg-08ea120e1da2c2a93)         │  │
        │  │  Publicly Accessible: NO         │  │
        │  └──────────────────────────────────┘  │
        └─────────────────────────────────────────┘
```
