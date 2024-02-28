# Domain Models

## Menu

```c#
class Menu
{
    Menu Create();
    void AddDinner(Dinner dinner);
    void RemoveDinner(Dinner diner);
    void UpdateSection(MenuSection section);
}

```

```json
{
  "id": "00000000-0000-0000-0000-0000",
  "name": "Something Menu",
  "description": "A menu with multiple types of foods",
  "averageRating": 4.5,
  "sections": [
    {
      "id": "00000000-0000-0000-0000-0000",
      "name": "Appetizers",
      "description": "Starters",
      "items": [
        {
          "id": "00000000-0000-0000-0000-0000",
          "name": "Fried Pickles",
          "description": "Deep fried pickles",
          "price": 5.99
        }
      ]
    }
  ],
  "createdDateTime": "2020-01-01T00:00:00.0000000Z",
  "UpdatedDateTime": "2020-01-01T00:00:00.0000000Z",

  "hostId": "00000000-0000-0000-0000-0000",
  "dinnerIds": ["00000000-0000-0000-0000-0000"],
  "menuReviewIds": ["00000000-0000-0000-0000-0000"]
}
```
