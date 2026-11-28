Feature: Authentication
  Protected pages are only available after signing in.
  Each role signs in through the real sign-in page and only sees its own menu items.

  @anonymous
  Scenario: Protected page redirects to the sign-in page when not signed in
    When I open the page "sentinel-entries" without signing in
    Then I am redirected to the sign-in page

  Scenario Outline: Sign in as <Role>
    When I sign in as "<Role>"
    Then I am signed in
    And the menu shows "<Menu item>"
    But the menu does not show "<Hidden menu item>"

    @guest
    Examples: Guest
      | Role  | Menu item  | Hidden menu item |
      | Guest | Startseite | Sentinel Daten   |

    @user
    Examples: User
      | Role | Menu item      | Hidden menu item  |
      | User | Sentinel Daten | Kryo Asservierung |

    @superuser
    Examples: Super user
      | Role      | Menu item         | Hidden menu item |
      | SuperUser | Kryo Asservierung | Administration   |

    @admin
    Examples: Administrator
      | Role  | Menu item      | Hidden menu item  |
      | Admin | Administration | Kryo Asservierung |

  @user
  Scenario: Sign out
    When I sign in as "User"
    And I sign out
    Then I am signed out
    And the menu does not show "Sentinel Daten"

  @user @admin
  Scenario: A new user is unlocked by assigning a laboratory
    Given the test user "User" has never signed in
    When I sign in as "User"
    Then I see the text "Eingeschränkter Gastzugang!"
    And the menu does not show "Sentinel Daten"
    When I am signed in as "Admin" in a new browser window
    And I open the page "admin"
    And I assign the organization "Testlabor Nord" to the test user "User"
    And I am signed in as "User" in a new browser window
    Then the menu shows "Sentinel Daten"
    And I do not see the text "Eingeschränkter Gastzugang!"
