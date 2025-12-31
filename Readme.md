# Sample Data Mapping Tool

**C# • WinForms • Data Cleansing • Selenium Automation • Background Processing**

A sanitized, portfolio-ready Windows Forms application that demonstrates how internal data-mapping and product-onboarding tools are built in real enterprise environments.

This project focuses on **data normalization**, **manual & automated workflows**, and **UI-driven automation**, while intentionally omitting proprietary schemas, credentials, and production endpoints.

---

## 🚀 Overview

The **Sample Data Mapping Tool** is designed to:

- Load raw part data from configurable database tables
- Clean, normalize, and de-duplicate part numbers
- Map parts to manufacturers and hierarchical categories
- Assist users with **manual review and enrichment**
- Automate external lookup workflows using Selenium (Shadow DOM)

This mirrors tooling commonly used in **inventory systems**, **e-commerce onboarding**, and **internal product catalogs**.

---

## 🧩 Key Features

### 🔹 Data Processing
- Manufacturer- and category-based filtering
- Duplicate row elimination
- Part number sanitization using regex
- Validation against existing database records
- Manual override and mapping workflows

### 🔹 UI-Driven Workflow
- WinForms interface with ComboBoxes, DataTables, and validation states
- Progress tracking for long-running operations
- Manual part review and navigation
- Visual feedback for duplicate or conflicting parts

### 🔹 Automation (Selenium)
- ChromeDriver-based automation
- Shadow DOM traversal using `GetShadowRoot()`
- Programmatic search submission
- Automated extraction and cleanup of response text

> ⚠️ **Note:**  
> Selenium selectors and automation flows are **representative only** and not tied to any proprietary or production system.

---

## 🛠 Technologies Used

- **C# / .NET Framework**
- **Windows Forms**
- **ADO.NET / DataTables**
- **Regular Expressions**
- **Selenium WebDriver**
- **ChromeDriver**
- **BackgroundWorker (async processing)**

---

## 🧩 Application Interface

This screenshot shows the main interface of the Part Data Mapping Tool. It includes part entry fields, manufacturer/category selectors, and Copilot-assisted search functionality.
![Main UI Overview](main/ui-overview.png)

---

## 🔐 Sanitization Notes

To make this project safe for public sharing:

- All database credentials and schemas are removed
- Real directory paths replaced with placeholders
- No real URLs or production systems referenced
- Business-specific rules generalized
- Sensitive identifiers renamed or omitted

The architecture, patterns, and logic remain authentic.

---

## 📌 What This Demonstrates to Employers

- Working with **legacy WinForms codebases**
- Cleaning and validating messy real-world data
- Designing internal tools for non-technical users
- Safely automating web UIs with Selenium
- Maintaining separation between UI, logic, and data access
- Writing production-style code with safety in mind

---

🔒 Read‑Only Usage Notice
This repository is provided solely for evaluation by potential employers.
You may view and review the code to assess engineering ability.

All other actions are not permitted, including:

Reuse or modification
Redistribution or publication
Integration into any software
Commercial or operational use
Automation of any real‑world systems
A full license file in this repository reinforces these restrictions.

---

📌 Notes
This code is intentionally incomplete and non‑functional.

Its purpose is to demonstrate architecture, coding style, and automation expertise, not to operate against any real software.

